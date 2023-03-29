/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Diagnostics;
using BardMusicPlayer.Transmogrify.Processor.Utilities;
using BardMusicPlayer.Transmogrify.Song.Config;
using BardMusicPlayer.Transmogrify.Song.Importers;
using BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro;
using BardMusicPlayer.Transmogrify.Song.Utilities;
using LiteDB;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Lyrics = BardMusicPlayer.Transmogrify.Song.Importers.LRC.Lyrics;
using Note = Melanchall.DryWetMidi.Interaction.Note;

namespace BardMusicPlayer.Transmogrify.Song;

public sealed class BmpSong
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
            ".gp"     => CovertMidiToSong(ImportGuitarPro.OpenGTPSongFile(path), path),
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

        //some midifiles have a ChannelPrefixEvent with a channel greater than 0xF. remove 'em.
        foreach (var chunk in midiFile.GetTrackChunks())
        {
            using var timedEventsManager = new TimedObjectsManager<TimedEvent>(chunk.Events);
            var events = timedEventsManager.Objects;
            var prefixList = events.Where(static e => e.Event is ChannelPrefixEvent).ToList();
            foreach (var tevent in prefixList.Where(tevent => (tevent.Event as ChannelPrefixEvent).Channel > 0xF))
                events.Remove(tevent);
        }

        return Task.FromResult(CovertMidiToSong(midiFile, name));
    }

    #region Import functions
    /// <summary>
    /// Open and process the midifile, tracks with note placed first
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

        //some midifiles have a ChannelPrefixEvent with a channel greater than 0xF. remove 'em.
        foreach (var chunk in midiFile.GetTrackChunks())
        {
            using var timedEventsManager = new TimedObjectsManager<TimedEvent>(chunk.Events);
            var events = timedEventsManager.Objects;
            var prefixList = events.Where(static e => e.Event is ChannelPrefixEvent).ToList();
            foreach (var tevent in prefixList.Where(tevent => (tevent.Event as ChannelPrefixEvent).Channel > 0xF))
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
        if (path.Substring(path.Length - 4).Equals(".mid"))
        {
            var fn = path.Substring(0, path.Length - 3);
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
