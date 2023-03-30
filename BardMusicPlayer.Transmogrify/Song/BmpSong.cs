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

public sealed partial class BmpSong
{
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

    #region Import functions
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
    }

    /// <summary>
    /// Get the preprocessed midi file for the sequencer
    /// </summary>
    /// <returns>MemoryStream</returns>
    public MemoryStream GetProcessedSequencerMidi()
    {
        var sourceMidi = TrackContainers.Values.SelectMany(static track => track.ConfigContainers).SelectMany(static track => track.Value.ProcessedTrackChunks);
        var midiFile = new MidiFile();

        //Create all tracks track
        var trackChunks = sourceMidi as TrackChunk[] ?? sourceMidi.ToArray();
        var allTracks = trackChunks.Merge();
        midiFile.Chunks.Add(allTracks);

        //Add the other tracks
        foreach (var track in trackChunks)
            midiFile.Chunks.Add(track);

        //set the channel numbers
        var index = 1;
        foreach (var track in midiFile.GetTrackChunks())
        {
            using (var manager = track.ManageTimedEvents())
            {
                var instrument = InstrumentTone.Parse(track.Events.OfType<SequenceTrackNameEvent>().First().Text.Split(':')[1]);
                var program = 0;
                foreach (var midiEvent in manager.Objects)
                {
                    switch (midiEvent.Event)
                    {
                        case NoteEvent ne:
                        {
                            //check if we have a guitar and create the change codes
                            if (instrument.Equals(InstrumentTone.ElectricGuitar) && index != 1)
                            {
                                var tp = instrument.GetInstrumentFromChannel(ne.Channel).MidiProgramChangeCode;
                                if (tp != program)
                                {
                                    var npc = new ProgramChangeEvent
                                    {
                                        ProgramNumber = (SevenBitNumber)tp,
                                        Channel       = (FourBitNumber)index,
                                        DeltaTime     = ne.DeltaTime
                                    };
                                    manager.Objects.Add(new TimedEvent(npc, midiEvent.Time));
                                    program = tp;
                                }
                            }
                            ne.Channel  = (FourBitNumber)index;
                            ne.Velocity = (SevenBitNumber)index;
                            break;
                        }
                        case ProgramChangeEvent pe:
                            pe.Channel = (FourBitNumber)index;
                            break;
                        //not needed?
                        case ControlChangeEvent ce:
                            ce.Channel = (FourBitNumber)index;
                            break;
                        case PitchBendEvent pbe:
                            pbe.Channel = (FourBitNumber)index;
                            break;
                    }
                }
                manager.SaveChanges();
            }
            index++;
            if (index == 16)
                break;
        }

        midiFile.ReplaceTempoMap(Tools.GetMsTempoMap());

        //cut to first note
        var delta = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(midiFile.GetTempoMap()).TotalMicroseconds / 1000;
        Parallel.ForEach(midiFile.GetTrackChunks(), chunk =>
        {
            var offset = Instrument.Parse(chunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text).SampleOffset; //get the offset
            using var manager = chunk.ManageTimedEvents();
            Parallel.ForEach(manager.Objects, midiEvent =>
            {
                if (midiEvent.Event.EventType is MidiEventType.NoteOn or MidiEventType.NoteOff or MidiEventType.ProgramChange or MidiEventType.Lyric)
                {
                    if (midiEvent.Time - delta < 0)
                    {
                        manager.Objects.Remove(midiEvent);
                    }
                    else
                        midiEvent.Time -= delta;
                }
            });
        });

        var stream = new MemoryStream();
        midiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { });
        stream.Flush();
        stream.Position = 0;

        return stream;
    }

    /// <summary>
    /// Get the midi file for the sequencer
    /// </summary>
    /// <returns>MemoryStream</returns>
    public MemoryStream GetDryWetSequencerMidi()
    {
        try
        {
            var c = TrackContainers.Values.Select(static tc => tc.SourceTrackChunk).ToList();

            var midiFile = new MidiFile(c);
            midiFile.ReplaceTempoMap(SourceTempoMap);

            Console.WriteLine("Scrubbing ");

            var loaderWatch = Stopwatch.StartNew();
            var newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();
            var firstNote = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(midiFile.GetTempoMap()).TotalMicroseconds / 1000;
            var firstNoteus = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(midiFile.GetTempoMap()).TotalMicroseconds;

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
                //Fix EndSpacing
                fixedNotes = fixedNotes.FixEndSpacing().Result;

                #region Tracknaming and octave shifting

                var octaveShift = 0;
                var trackName = originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text ?? "";
                var oTrackName = trackName;

                var rex = MyRegex();
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
                if (oTrackName.StartsWith("Lyrics:"))
                    trackName = oTrackName;

                var newChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                #endregion Tracknaming and octave shifting

                //Create Progchange Event if no IgnoreProgChange is set
                if (!BmpPigeonhole.Instance.IgnoreProgChange || oTrackName.Contains("Program:ElectricGuitar"))
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


            var stream = new MemoryStream();

            using (var manager = new TimedObjectsManager<TimedEvent>(newMidiFile.GetTrackChunks().First().Events))
                manager.Objects.Add(new TimedEvent(new MarkerEvent(), newMidiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000));

            newMidiFile.Write(stream, settings: new WritingSettings
            {
                TextEncoding = Encoding.UTF8
            });

            stream.Flush();
            stream.Position = 0;

            loaderWatch.Stop();
            Console.WriteLine("Scrubbing MS: " + loaderWatch.ElapsedMilliseconds);
            return stream;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// deprecated loader
    /// </summary>
    /// <returns>MemoryStream</returns>
    public MemoryStream GetDryWetSequencerMidi_()
    {
        try
        {
            var c = TrackContainers.Values.Select(static tc => tc.SourceTrackChunk).ToList();

            var midiFile = new MidiFile(c);
            midiFile.ReplaceTempoMap(SourceTempoMap);

            Console.WriteLine("Scrubbing ");
            var loaderWatch = Stopwatch.StartNew();
            var newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();
            var tempoMap = midiFile.GetTempoMap().Clone();
            var firstNote = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;
            var firstNoteus = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds;
            var originalTrackChunks = new List<TrackChunk>();

            var allTracks = new TrackChunk(new SequenceTrackNameEvent("None"));
            allTracks.AddObjects(midiFile.GetNotes());

            using (var manager = allTracks.ManageTimedEvents())
            {
                manager.Objects.RemoveAll(e => e.Event.EventType != MidiEventType.NoteOn &&
                                               e.Event.EventType != MidiEventType.NoteOff &&
                                               e.Event.EventType != MidiEventType.SequenceTrackName);
                manager.SaveChanges();
            }
            midiFile.Chunks.Add(allTracks);


            Parallel.ForEach(originalTrackChunks.Where(static x => x.GetNotes().Any() || x.Events.OfType<LyricEvent>().Any() ), (originalChunk, loopState, index) =>
            {
                var watch = Stopwatch.StartNew();
                var tempoMap = midiFile.GetTempoMap().Clone();
                var noteVelocity = int.Parse(index.ToString()) + 1;

                var allNoteEvents = new Dictionary<int, Dictionary<long, Note>>();
                for (var i = 0; i < 128; i++) allNoteEvents.Add(i, new Dictionary<long, Note>());

                foreach (var note in originalChunk.GetNotes())
                {
                    long noteOnMS;
                    long noteOffMS;

                    try
                    {
                        noteOnMS = 5000000 + note.GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds - firstNoteus;
                        noteOffMS = 5000000 + note.GetTimedNoteOffEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds - firstNoteus;
                    }
                    catch (Exception) { continue; }
                    int noteNumber = note.NoteNumber;

                    var newNote = new Note((SevenBitNumber)noteNumber,
                        time: noteOnMS /1000,
                        length: noteOffMS /1000 - noteOnMS/1000
                    )
                    {
                        Channel     = (FourBitNumber)0,
                        Velocity    = (SevenBitNumber)noteVelocity,
                        OffVelocity = (SevenBitNumber)noteVelocity
                    };

                    if (allNoteEvents[noteNumber].TryGetValue(noteOnMS, out var value))
                    {
                        if (value.Length < note.Length) allNoteEvents[noteNumber][noteOnMS] = newNote;
                    }
                    else allNoteEvents[noteNumber].Add(noteOnMS, newNote);
                }
                watch.Stop();

                Debug.WriteLine("step 1: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                watch = Stopwatch.StartNew();

                var newChunk = new TrackChunk();
                for (var i = 0; i < 128; i++)
                {
                    long lastNoteTimeStamp = -1;
                    foreach (var noteEvent in allNoteEvents[i])
                    {
                        if (lastNoteTimeStamp >= 0 && allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp >= noteEvent.Key)
                            allNoteEvents[i][lastNoteTimeStamp].Length -= allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp + 1 - noteEvent.Key;

                        lastNoteTimeStamp = noteEvent.Key;
                    }
                }
                newChunk.AddObjects(allNoteEvents.SelectMany(static s => s.Value).Select(static s => s.Value).ToArray());
                allNoteEvents = null;
                watch.Stop();
                Debug.WriteLine("step 2 [Fix Chords]: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                watch = Stopwatch.StartNew();

                var notesToFix = newChunk.GetNotes().Reverse().ToArray();
                for (var i = 1; i < notesToFix.Length; i++)
                {
                    int noteNum = notesToFix[i].NoteNumber;
                    var time = notesToFix[i].GetTimedNoteOnEvent().Time;
                    var dur = notesToFix[i].Length;
                    int velocity = notesToFix[i].Velocity;

                    var lowestParent = notesToFix[0].GetTimedNoteOnEvent().Time;
                    for (var k = i - 1; k >= 0; k--)
                    {
                        var lastOn = notesToFix[k].GetTimedNoteOnEvent().Time;
                        if (lastOn < lowestParent) lowestParent = lastOn;
                    }
                    if (lowestParent <= time + 30)
                    {
                        time = lowestParent - 30;
                        if (time < 0) continue;
                        notesToFix[i].Time   = time;
                        dur                  = 25;
                        notesToFix[i].Length = dur;
                    }
                }

                watch.Stop();
                Debug.WriteLine("step 3: " + noteVelocity + ": " + watch.ElapsedMilliseconds);

                #region calc shortest note
                watch = Stopwatch.StartNew();

                notesToFix = notesToFix.Reverse().ToArray();
                var fixedNotes = new List<Note>();
                for (var j = 0; j < notesToFix.Length; j++)
                {
                    var noteNum = notesToFix[j].NoteNumber;
                    var time = notesToFix[j].Time;
                    var dur = notesToFix[j].Length;
                    var channel = notesToFix[j].Channel;
                    var velocity = notesToFix[j].Velocity;

                    if (j + 1 < notesToFix.Length)
                    {
                        switch (notesToFix[j].Length)
                        {
                            // MEOWCHESTRA
                            // Bandaid fix: If sustained note is 100ms or greater, ensure 60ms between the end of that note and the beginning of the next note.
                            // Otherwise, leave the behavior as it was before.
                            case >= 100 when notesToFix[j + 1].Time <= notesToFix[j].Time + notesToFix[j].Length + 60:
                                dur = notesToFix[j + 1].Time - notesToFix[j].Time - 60;
                                dur = dur < 60 ? 60 : dur;
                                break;
                            case < 100 when notesToFix[j + 1].Time <= notesToFix[j].Time + notesToFix[j].Length + 25:
                                dur = notesToFix[j + 1].Time - notesToFix[j].Time - 25;
                                dur = dur < 25 ? 25 : dur;
                                break;
                        }
                    }

                    fixedNotes.Add(new Note(noteNum, dur, time)
                    {
                        Channel     = channel,
                        Velocity    = velocity,
                        OffVelocity = velocity
                    });
                }
                notesToFix = null;

                watch.Stop();
                Debug.WriteLine("step 4: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                #endregion

                #region Tracknaming and octave shifting
                watch = Stopwatch.StartNew();

                var octaveShift = 0;
                var trackName = originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text ?? "";
                var oTrackName = trackName;

                var rex = MyRegex();
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
                if (oTrackName.StartsWith("Lyrics:"))
                    trackName = oTrackName;

                newChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                #endregion Tracknaming and octave shifting

                //Create Progchange Event if no IgnoreProgChange is set
                if (!BmpPigeonhole.Instance.IgnoreProgChange || oTrackName.Contains("Program:ElectricGuitar"))
                {
                    foreach (var timedEvent in originalChunk.GetTimedEvents())
                    {
                        if (timedEvent.Event is not ProgramChangeEvent programChangeEvent)
                            continue;
                        //Skip all except guitar | implement if we need this again
                        if (programChangeEvent.ProgramNumber < 27 || programChangeEvent.ProgramNumber > 31)
                            continue;

                        var channel = programChangeEvent.Channel;
                        using var manager = new TimedObjectsManager(newChunk.Events, ObjectType.TimedEvent | ObjectType.Note);
                        var timedEvents = manager.Objects;
                        if (5000 + timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 - firstNote < 0)
                            timedEvents.Add(new TimedEvent(new ProgramChangeEvent(programChangeEvent.ProgramNumber), 5000));
                        else
                            timedEvents.Add(new TimedEvent(new ProgramChangeEvent(programChangeEvent.ProgramNumber), 5000 + timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 - firstNote/* Absolute time too */));
                    }
                }
                //Create lyrics from midi
                foreach (var timedEvent in originalChunk.GetTimedEvents())
                {
                    if (timedEvent.Event is not LyricEvent lyricsEvent)
                        continue;

                    using var manager = new TimedObjectsManager(newChunk.Events, ObjectType.TimedEvent | ObjectType.Note);
                    var timedEvents = manager.Objects;
                    if (5000 + timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 - firstNote < 5000)
                        timedEvents.Add(new TimedEvent(new LyricEvent(lyricsEvent.Text), 5000));
                    else
                        timedEvents.Add(new TimedEvent(new LyricEvent(lyricsEvent.Text), 5000 + timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 - firstNote));
                }

                //Create Aftertouch Event
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

                watch.Stop();
                Debug.WriteLine("step 5: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                watch = Stopwatch.StartNew();

                newTrackChunks.TryAdd(noteVelocity, newChunk);

                watch.Stop();
                Debug.WriteLine("step 6: " + noteVelocity + ": " + watch.ElapsedMilliseconds);

            });

            var newMidiFile = new MidiFile();
            newTrackChunks.TryRemove(newTrackChunks.Count, out var trackZero);
            newMidiFile.Chunks.Add(trackZero);
            newMidiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(375);
            using (var tempoManager = newMidiFile.ManageTempoMap()) 
                tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(160));

            newMidiFile.Chunks.AddRange(newTrackChunks.Values);
                


            tempoMap = newMidiFile.GetTempoMap();
            var delta = newMidiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;

            Parallel.ForEach(newMidiFile.GetTrackChunks(), chunk =>
            {
                var offset = Instrument.Parse(chunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text).SampleOffset; //get the offset
                /*using (var notesManager = chunk.ManageNotes())
                {
                    foreach (Note note in notesManager.Notes)
                    {
                        long newStart = note.Time + offset - delta;
                        note.Time = newStart;
                    }
                }*/
                using var manager = chunk.ManageTimedEvents();
                foreach (var _event in manager.Objects)
                {
                    var noteEvent = _event.Event as NoteEvent;
                    var programChangeEvent = _event.Event as ProgramChangeEvent;
                    var lyricsEvent = _event.Event as LyricEvent;

                    //Note alignment
                    if (noteEvent != null)
                    {
                        var newStart = _event.Time + offset - delta;
                        _event.Time = newStart;
                    }

                    //Prog alignment
                    if (programChangeEvent != null)
                    {
                        var newStart = _event.Time + offset - delta;
                        if (newStart <= -1)
                            manager.Objects.Remove(_event);
                        else
                            _event.Time = newStart;

                        //if theres a new offset, use this one
                        if (programChangeEvent.ProgramNumber >=27 && programChangeEvent.ProgramNumber <= 31)
                            offset = Instrument.ParseByProgramChange(programChangeEvent.ProgramNumber).SampleOffset;
                    }

                    //and lyrics
                    if (lyricsEvent != null)
                    {

                    }

                }

                foreach (var _event in manager.Objects)
                {
                    var lyricsEvent = _event.Event as LyricEvent;
                    if (lyricsEvent == null)
                        continue;

                    var newStart = _event.Time - delta;
                    if (newStart <= -1)
                        manager.Objects.Remove(_event);
                    else
                        _event.Time = newStart;
                }

                /*foreach (TimedEvent _event in manager.Events)
                    {
                        var programChangeEvent = _event.Event as ChannelAftertouchEvent;
                        if (programChangeEvent == null)
                            continue;

                        long newStart = _event.Time - delta;
                        if (newStart <= -1)
                            manager.Events.Remove(_event);
                        else
                            _event.Time = newStart;
                    }*/
            });

            //Append the lyrics from the lrc
            var lrcTrack = new TrackChunk(new SequenceTrackNameEvent("Lyrics: "));
            using (var manager = new TimedObjectsManager(lrcTrack.Events, ObjectType.TimedEvent | ObjectType.Note))
            {
                var timedEvents = manager.Objects;
                foreach (var line in LyricsContainer)
                {
                    var timedEvent = new TimedEvent(new LyricEvent(line.Value)) as ITimedObject;
                    timedEvent.SetTime(new MetricTimeSpan(line.Key.Hour, line.Key.Minute, line.Key.Second, line.Key.Millisecond), tempoMap);
                    timedEvents.Add(timedEvent);
                }
            }
            newMidiFile.Chunks.Add(lrcTrack);


            var stream = new MemoryStream();

            using (var manager = new TimedObjectsManager<TimedEvent>(newMidiFile.GetTrackChunks().First().Events))
                manager.Objects.Add(new TimedEvent(new MarkerEvent(), newMidiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000));

            newMidiFile.Write(stream, MidiFileFormat.MultiTrack, settings: new WritingSettings
            {
                TextEncoding = Encoding.UTF8
            });

            stream.Flush();
            stream.Position = 0;

            loaderWatch.Stop();
            Console.WriteLine("Scrubbing MS: " + loaderWatch.ElapsedMilliseconds);
            return stream;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
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

        midiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { });
        stream.Flush();
        stream.Position = 0;

        return stream;
    }

    [GeneratedRegex("^([A-Za-z _]+)([-+]\\d)?")]
    private static partial Regex MyRegex();
}