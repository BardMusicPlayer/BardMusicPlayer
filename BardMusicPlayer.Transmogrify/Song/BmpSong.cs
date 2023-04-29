/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Processor.Utilities;
using BardMusicPlayer.Transmogrify.Song.Config;
using BardMusicPlayer.Transmogrify.Song.Importers;
using BardMusicPlayer.Transmogrify.Song.Importers.LRC;
using BardMusicPlayer.Transmogrify.Song.Utilities;
using LiteDB;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Song;

public sealed class BmpSong
{
    #region LiteDB accs
    /// <summary>
    /// 
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// the internal title / showed in playlist
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The displayed title in chat
    /// </summary>
    public string DisplayedTitle { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// TempoMap
    /// </summary>
    public TempoMap SourceTempoMap { get; set; } = TempoMap.Default;

    /// <summary>
    /// TrackContainer
    /// </summary>
    public Dictionary<long, TrackContainer> TrackContainers { get; set; } = new();

    /// <summary>
    /// Lyrics
    /// </summary>
    public Dictionary<DateTime, string> LyricsContainer { get; set; } = new();

    /// <summary>
    /// Song duration
    /// </summary>
    public TimeSpan Duration { get; set; }

    #endregion

    [BsonIgnore]
    public MidiFile cachedSequencerMidi { get; set; } = null;

    #region Import functions

    /// <summary>
    /// File Path
    /// </summary>
    public string FilePath { get; set; }
    
    /// <summary>
    /// opens a file and selects the processing by file ext.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Task<BmpSong> OpenFile(string path)
    {
        var song = Path.GetExtension(path) switch
        {
            ".mmsong" => CovertMidiToSong(MMSongImporter.OpenMMSongFile(path), path),
            ".mml"    => CovertMidiToSong(MMLSongImporter.OpenMMLSongFile(path), path),
            ".gp"     => CovertMidiToSong(Importers.GuitarPro.ImportGuitarPro.OpenGTPSongFile(path), path),
            _         => OpenMidiFile(path)
        };
        song.FilePath = path;
        song.PrepareCachedSequencerMidi();
        return Task.FromResult(song);
    }

    /// <summary>
    /// Open and process the mididata as byte[], tracks with note placed first
    /// </summary>
    /// <param name="data"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Task<BmpSong> ImportMidiFromByte(byte[] data, string name)
    {
        var memoryStream = new MemoryStream();
        memoryStream.Write(data, 0, data.Length);
        memoryStream.Position = 0;
        var midiFile = memoryStream.ReadAsMidiFile();
        memoryStream.Dispose();

        //some midi files have a ChannelPrefixEvent with a channel greater than 0xF. remove 'em.
        foreach (var chunk in midiFile.GetTrackChunks())
        {
            using var timedEventsManager = new TimedObjectsManager<TimedEvent>(chunk.Events);
            var events = timedEventsManager.Objects;
            var prefixList = events.Where(static e => e.Event is ChannelPrefixEvent).ToList();
            foreach (var tevent in prefixList.Where(tevent => ((ChannelPrefixEvent)tevent.Event).Channel > 0xF))
                events.Remove(tevent);
        }

        return Task.FromResult(CovertMidiToSong(midiFile, name));
    }

    /// <summary>
    /// Open and process the midi file, tracks with note placed first
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static BmpSong OpenMidiFile(string path)
    {
        if (!File.Exists(path))
            throw new BmpTransmogrifyException("File " + path + " does not exist!");

        using var fileStream = File.OpenRead(path);
        var midiFile = fileStream.ReadAsMidiFile();
        fileStream.Dispose();

        //some midi files have a ChannelPrefixEvent with a channel greater than 0xF. remove 'em.
        foreach (var chunk in midiFile.GetTrackChunks())
        {
            using var timedEventsManager = new TimedObjectsManager<TimedEvent>(chunk.Events);
            var events = timedEventsManager.Objects;
            var prefixList = events.Where(static e => e.Event is ChannelPrefixEvent).ToList();
            foreach (var tevent in prefixList.Where(tevent => ((ChannelPrefixEvent)tevent.Event).Channel > 0xF))
                events.Remove(tevent);
        }

        var song = CovertMidiToSong(midiFile, path);
        song.FilePath = path;
        return song;
    }
    #endregion

    /// <summary>
    /// convert an imported file to a BmpSong
    /// </summary>
    /// <param name="midiFile"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private static BmpSong CovertMidiToSong(MidiFile midiFile, string path)
    {
        var tempoMap = midiFile.GetTempoMap();
        TimeSpan midiFileDuration = midiFile.GetTimedEvents().LastOrDefault(static e => e.Event is NoteOffEvent)?.TimeAs<MetricTimeSpan>(tempoMap) ?? new MetricTimeSpan();

        var timer = new Stopwatch();
        timer.Start();

        var song = new BmpSong
        {
            Title           = Path.GetFileNameWithoutExtension(path),
            SourceTempoMap  = midiFile.GetTempoMap().Clone(),
            TrackContainers = new Dictionary<long, TrackContainer>(),
            Duration        = midiFileDuration
        };

        //Get the lrc file for the midi, if there's any
        if (path[^4..].Equals(".mid"))
        {
            var fn = path[..^3];
            if (File.Exists(fn + "lrc"))
            {
                var t = Lyrics.Parse(File.ReadAllText(fn + "lrc"));
                song.DisplayedTitle = t.Lyrics.MetaData.Title;

                foreach (var line in t.Lyrics.Lines)
                {
                    if (!song.LyricsContainer.TryGetValue(line.Timestamp, out var content))
                    {
                        content = "";
                    }

                    content                              += line.Content + Environment.NewLine;
                    song.LyricsContainer[line.Timestamp] =  content;
                }
            }
        }

        var trackChunkArray = midiFile.GetTrackChunks().ToArray();

        //Set note tracks at first
        var skippedTracks = new List<int>();
        var index = 0;
        for (var i = 0; i < midiFile.GetTrackChunks().Count(); i++)
        {
            //ignore tracks without notes
            if (trackChunkArray[i].ManageNotes().Objects.Any())
            {
                song.TrackContainers[index] = new TrackContainer { SourceTrackChunk = (TrackChunk)trackChunkArray[i].Clone() };
                index++;
            }
            else
                skippedTracks.Add(i);
        }
        //set the ignored tracks for data
        foreach (var i in skippedTracks)
        {
            song.TrackContainers[index] = new TrackContainer { SourceTrackChunk = (TrackChunk)trackChunkArray[i].Clone() };
            index++;
        }

        //check the tracks for data
        for (var i = 0; i < song.TrackContainers.Count; i++)
        {
            song.TrackContainers[i].ConfigContainers = song.TrackContainers[i].SourceTrackChunk.ReadConfigs(i, song);
        }
        //process the tracks we've got
        Parallel.For(0, song.TrackContainers.Count, i =>
        {
            Parallel.For(0, song.TrackContainers[i].ConfigContainers.Count, async j =>
            {
                switch (song.TrackContainers[i].ConfigContainers[j].ProcessorConfig)
                {
                    case ClassicProcessorConfig classicConfig:
                        Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                          classicConfig.GetType() +
                                          " Instrument:" + classicConfig.Instrument + " OctaveRange:" +
                                          classicConfig.OctaveRange + " PlayerCount:" + classicConfig.PlayerCount +
                                          " IncludeTracks:" + string.Join(",", classicConfig.IncludedTracks));
                        song.TrackContainers[i].ConfigContainers[j].ProcessedTrackChunks =
                            await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                        break;
                    case LyricProcessorConfig lyricConfig:
                        Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                          lyricConfig.GetType() + " PlayerCount:" + lyricConfig.PlayerCount +
                                          " IncludeTracks:" + string.Join(",", lyricConfig.IncludedTracks));
                        song.TrackContainers[i].ConfigContainers[j].ProcessedTrackChunks =
                            await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                        break;
                    case VSTProcessorConfig vstConfig:
                        Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                          vstConfig.GetType() + " PlayerCount:" + vstConfig.PlayerCount +
                                          " IncludeTracks:" + string.Join(",", vstConfig.IncludedTracks));
                        song.TrackContainers[i].ConfigContainers[j].ProcessedTrackChunks =
                            await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                        break;
                    default:
                        Console.WriteLine("error unknown config.");
                        break;
                }
            });
        });
        skippedTracks.Clear();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));

        return song;
    }

    /// <summary>
    /// Prepare the Midi for the sequencer and siren
    /// </summary>
    public void PrepareCachedSequencerMidi()
    {
        try
        {
            var c = TrackContainers.Values.Select(static tc => tc.SourceTrackChunk).ToList();

            var midiFile = new MidiFile(c);
            midiFile.ReplaceTempoMap(SourceTempoMap);

            Console.WriteLine("Scrubbing ");
            var loaderWatch = Stopwatch.StartNew();

            var newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();
            var firstNoteus = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(midiFile.GetTempoMap()).TotalMicroseconds;
            var firstNote = firstNoteus / 1000;

            var allTracks = new TrackChunk(new SequenceTrackNameEvent("None"));
            allTracks.AddObjects(midiFile.GetNotes());
            midiFile.Chunks.Add(allTracks);

            Parallel.ForEach(midiFile.GetTrackChunks().Where(static x => x.GetNotes().Any() || x.Events.OfType<LyricEvent>().Any()), (originalChunk, loopState, index) =>
            {
                var tempoMap = midiFile.GetTempoMap().Clone();
                var noteVelocity = int.Parse(index.ToString()) + 1;

                //Generate NoteDict
                var allNoteEvents = Extensions.GetNoteDictionary(originalChunk, tempoMap, firstNoteus, noteVelocity).Result;

                // only a few midis trigger this, leave it or delete it?
                // actually just a waste
                /*for (int i = 0; i < 128; i++)
                {
                    long lastNoteTimeStamp = -1;
                    foreach (var noteEvent in allNoteEvents[i])
                    {
                        if (lastNoteTimeStamp >= 0 && allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp >= noteEvent.Key)
                            allNoteEvents[i][lastNoteTimeStamp].Length -= allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp + 1 - noteEvent.Key;
                        lastNoteTimeStamp = noteEvent.Key;
                    }
                }*/

                //Fix Chords
                var fixedNotes = allNoteEvents.SelectMany(static s => s.Value).Select(static s => s.Value).ToList().FixChords(30).Result;
                allNoteEvents = null;
                //Fix EndSpacing
                fixedNotes = fixedNotes.FixEndSpacing().Result;

                #region Tracknaming and octave shifting

                var octaveShift = 0;
                var trackName = originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text ?? "";
                var o_trackName = trackName;

                var rex = new Regex(@"^([A-Za-z _]+)([-+]\d)?");
                if (rex.Match(trackName) is { } match)
                {
                    if (!string.IsNullOrEmpty(match.Groups[1].Value))
                    {
                        trackName = Instrument.Parse(match.Groups[1].Value).Name;
                        if (!string.IsNullOrEmpty(match.Groups[2].Value))
                            if (int.TryParse(match.Groups[2].Value, out var os))
                                octaveShift = os;

                        trackName = octaveShift switch
                        {
                            > 0 => trackName + "+" + octaveShift,
                            < 0 => trackName + octaveShift,
                            _   => trackName
                        };
                    }

                    //last try with the program number
                    if (string.IsNullOrEmpty(match.Groups[1].Value) || trackName.Equals("Unknown") || trackName.Equals("None"))
                    {
                        var prog = originalChunk.Events.OfType<ProgramChangeEvent>().FirstOrDefault();
                        if (prog != null)
                            trackName = Instrument.ParseByProgramChange(prog.ProgramNumber).Name;
                    }

                }
                //If we have a lyrics tracks
                if (o_trackName.StartsWith("Lyrics:"))
                    trackName = o_trackName;

                var newChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                #endregion Tracknaming and octave shifting

                //Create Progchange Event if no IgnoreProgChange is set
                if (!BmpPigeonhole.Instance.IgnoreProgChange || o_trackName.Contains("Program:ElectricGuitar"))
                    newChunk.AddObjects(Extensions.AddProgramChangeEvents(originalChunk, tempoMap, firstNote).Result.GetTimedEvents());

                //Add the lyrics
                newChunk.AddObjects(Extensions.AddLyricsEvents(originalChunk, tempoMap, firstNote).Result.GetTimedEvents());

                //Create Aftertouch Event - maybe for some special things
                /*foreach (var timedEvent in originalChunk.GetTimedEvents())
                {
                    var programChangeEvent = timedEvent.Event as ChannelAftertouchEvent;
                    if (programChangeEvent == null)
                        continue;
                    var channel = programChangeEvent.Channel;
                    using (var manager = new TimedEventsManager(newChunk.Events))
                    {
                        TimedEventsCollection timedEvents = manager.Events;
                        timedEvents.Add(new TimedEvent(new ChannelAftertouchEvent(programChangeEvent.AftertouchValue), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote));
                    }
                }*/
                newChunk.AddObjects(fixedNotes);
                newTrackChunks.TryAdd(noteVelocity, newChunk);
            });

            var newMidiFile = new MidiFile();
            newTrackChunks.TryRemove(newTrackChunks.Count, out var trackZero);
            newMidiFile.Chunks.Add(trackZero);
            newMidiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(375);
            using (var tempoManager = newMidiFile.ManageTempoMap())
                tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(160));

            newMidiFile.Chunks.AddRange(newTrackChunks.Values);

            //realign the events
            var delta = newMidiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(newMidiFile.GetTempoMap()).TotalMicroseconds / 1000;
            Parallel.ForEach(newMidiFile.GetTrackChunks(), chunk =>
            {
                chunk = Extensions.RealignTrackEvents(chunk, delta).Result;
            });

            //Append the lyrics from the lrc
            var lrcTrack = new TrackChunk(new SequenceTrackNameEvent("Lyrics: "));
            using (var manager = new TimedObjectsManager(lrcTrack.Events, ObjectType.TimedEvent | ObjectType.Note))
            {
                var timedEvents = manager.Objects;
                foreach (var line in LyricsContainer)
                {
                    var timedEvent = new TimedEvent(new LyricEvent(line.Value)) as ITimedObject;
                    timedEvent.SetTime(new MetricTimeSpan(line.Key.Hour, line.Key.Minute, line.Key.Second, line.Key.Millisecond), newMidiFile.GetTempoMap());
                    timedEvents.Add(timedEvent);
                }
            }
            newMidiFile.Chunks.Add(lrcTrack);

            using (var manager = new TimedObjectsManager<TimedEvent>(newMidiFile.GetTrackChunks().First().Events))
                manager.Objects.Add(new TimedEvent(new MarkerEvent(), newMidiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000));

            cachedSequencerMidi = newMidiFile;

            loaderWatch.Stop();
            Console.WriteLine("Scrubbing MS: " + loaderWatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    ///  Get the processed midi file
    /// </summary>
    /// <returns></returns>
    public Task<MidiFile> GetProcessedMidiFile()
    {
        if (cachedSequencerMidi == null)
            PrepareCachedSequencerMidi();
        return Task.FromResult(cachedSequencerMidi);
    }

    /// <summary>
    /// Get the midi file for the sequencer
    /// </summary>
    /// <returns>MemoryStream</returns>
    public MemoryStream GetDryWetSequencerMidi()
    {
        var outStream = new MemoryStream();
        if (cachedSequencerMidi == null)
            PrepareCachedSequencerMidi();

        cachedSequencerMidi?.Write(outStream, settings: new WritingSettings
        {
            TextEncoding = Encoding.UTF8
        });

        outStream.Flush();
        outStream.Position = 0;
        return outStream;
    }

    /// <summary>
    /// Exports the song to a midi file
    /// </summary>
    /// <returns></returns>
    public MemoryStream GetExportMidi()
    {
        var c = TrackContainers.Values.Select(static tc => tc.SourceTrackChunk).ToList();

        var midiFile = new MidiFile(c);
        midiFile.ReplaceTempoMap(SourceTempoMap);

        var stream = new MemoryStream();

        using (var manager = new TimedObjectsManager<TimedEvent>(midiFile.GetTrackChunks().First().Events))
            manager.Objects.Add(new TimedEvent(new MarkerEvent(), midiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000));

        midiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings());
        stream.Flush();
        stream.Position = 0;

        return stream;
    }
}

/*
/// <summary>
/// 
/// </summary>
/// <returns></returns>
public Task<MidiFile> GetProcessedMidiFile()
{
    var sourceMidiData = new MidiFile(TrackContainers.Values.SelectMany(static track => track.ConfigContainers).SelectMany(static track => track.Value.ProcessedTrackChunks));
    sourceMidiData.ReplaceTempoMap(Tools.GetMsTempoMap());
    var midiFile = new MidiFile();
    if (sourceMidiData.GetNotes().Count < 1) return Task.FromResult(midiFile);
    var delta = sourceMidiData.GetNotes().First().Time;
    foreach (var trackChunk in sourceMidiData.GetTrackChunks())
    {
        var trackName = trackChunk.Events.OfType<SequenceTrackNameEvent>().First().Text;
        if (trackName.StartsWith("tone:", StringComparison.Ordinal))
        {
            var newTrackChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
            var newNotes = new List<Note>();
            foreach (var note in trackChunk.GetNotes().Where(note => note.Time - delta >= 0))
            {
                note.Time -= delta;
                newNotes.Add(note);
            }
            newTrackChunk.AddObjects(newNotes);
            midiFile.Chunks.Add(newTrackChunk);
        }
        else if (trackName.StartsWith("lyric:", StringComparison.Ordinal))
        {
            var newTrackChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
            var newLyrics = new List<TimedEvent>();
            foreach (var midiEvent in trackChunk.GetTimedEvents()
                        .Where(static e => e.Event.EventType == MidiEventType.Lyric)
                        .Where(midiEvent => midiEvent.Time - delta >= 0))
            {
                midiEvent.Time -= delta;
                newLyrics.Add(midiEvent);
            }
            newTrackChunk.AddObjects(newLyrics);
            midiFile.Chunks.Add(newTrackChunk);
        }
    }
    midiFile.ReplaceTempoMap(Tools.GetMsTempoMap());
    return Task.FromResult(midiFile);
}*/

/*
/// <summary>
/// Get the preprocessed midi file for the sequencer
/// </summary>
/// <returns>MemoryStream</returns>
public MemoryStream GetProcessedSequencerMidi()
{
    var sourceMidi = TrackContainers.Values.SelectMany(static track => track.ConfigContainers).SelectMany(static track => track.Value.ProcessedTrackChunks);
    var midiFile = new MidiFile();
    //Create all tracks track
    TrackChunk allTracks = Melanchall.DryWetMidi.Core.TrackChunkUtilities.Merge(sourceMidi);
    midiFile.Chunks.Add(allTracks);
    //Add the other tracks
    foreach (TrackChunk track in sourceMidi)
        midiFile.Chunks.Add(track);
    //set the channel numbers
    int index = 1;
    foreach (TrackChunk track in midiFile.GetTrackChunks())
    {
        using (var manager = track.ManageTimedEvents())
        {
            InstrumentTone instrument = InstrumentTone.Parse(track.Events.OfType<SequenceTrackNameEvent>().First().Text.Split(':')[1]);
            int Program = 0;
            foreach (TimedEvent midiEvent in manager.Objects)
            {
                if (midiEvent.Event is NoteEvent ne)
                {
                    //check if we have a guitar and create the change codes
                    if (instrument.Equals(InstrumentTone.ElectricGuitar) && (index != 1))
                    {
                        int tp = instrument.GetInstrumentFromChannel(ne.Channel).MidiProgramChangeCode;
                        if (tp != Program)
                        {
                            ProgramChangeEvent npc = new ProgramChangeEvent();
                            npc.ProgramNumber = (SevenBitNumber)tp;
                            npc.Channel = (FourBitNumber)index;
                            npc.DeltaTime = ne.DeltaTime;
                            manager.Objects.Add(new TimedEvent(npc, midiEvent.Time));
                            Program = tp;
                        }
                    }
                    ne.Channel = (FourBitNumber)index;
                    ne.Velocity = (SevenBitNumber)index;
                }
                if (midiEvent.Event is ProgramChangeEvent pe)
                    pe.Channel = (FourBitNumber)index;
                //not needed?
                if (midiEvent.Event is ControlChangeEvent ce)
                    ce.Channel = (FourBitNumber)index;
                if (midiEvent.Event is PitchBendEvent pbe)
                    pbe.Channel = (FourBitNumber)index;
            }
            manager.SaveChanges();
        }
        index++;
        if (index == 16)
            break;
    }
    midiFile.ReplaceTempoMap(Tools.GetMsTempoMap());
    //cut to first note
    long delta = (midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(midiFile.GetTempoMap()).TotalMicroseconds / 1000);
    Parallel.ForEach(midiFile.GetTrackChunks(), chunk =>
    {
        int offset = Instrument.Parse(chunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text).SampleOffset; //get the offset
        using (var manager = chunk.ManageTimedEvents())
        {
            Parallel.ForEach(manager.Objects, midiEvent =>
            {
                if ((midiEvent.Event.EventType == MidiEventType.NoteOn) ||
                    (midiEvent.Event.EventType == MidiEventType.NoteOff) ||
                    (midiEvent.Event.EventType == MidiEventType.ProgramChange) ||
                    (midiEvent.Event.EventType == MidiEventType.Lyric))
                {
                    if (midiEvent.Time - delta < 0)
                        manager.Objects.Remove(midiEvent);
                    else
                        midiEvent.Time -= delta;
                }
            });
        }
    });
    var stream = new MemoryStream();
    midiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { });
    stream.Flush();
    stream.Position = 0;
    return stream;
}*/