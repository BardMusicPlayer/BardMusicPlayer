/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Processor.Utilities;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Transmogrify.Song.Config;
using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Core;
using BardMusicPlayer.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Processor;

internal class ClassicProcessor : BaseProcessor
{
    public ClassicProcessorConfig ProcessorConfig { get; set; }

    internal ClassicProcessor(ClassicProcessorConfig processorConfig, BmpSong song) : base(song)
    {
        ProcessorConfig = processorConfig;
    }

    public override async Task<List<TrackChunk>> Process()
    {
        var trackChunks = new List<TrackChunk> { Song.TrackContainers[ProcessorConfig.Track].SourceTrackChunk }.Concat(ProcessorConfig.IncludedTracks.Select(track => Song.TrackContainers[track].SourceTrackChunk)).ToList();

        //convert progchanges to lower notes, if it's a guitar
        if (ProcessorConfig.Instrument.InstrumentTone.Index == InstrumentTone.ElectricGuitar.Index)
        {

            foreach (var timedEvent in trackChunks.GetTimedEvents())
            {
                if (timedEvent.Event is not ProgramChangeEvent programChangeEvent)
                    continue;

                //Skip all except guitar
                if (programChangeEvent.ProgramNumber < 27 || programChangeEvent.ProgramNumber > 31)
                    continue;

                var number = (int)Instrument.ParseByProgramChange(programChangeEvent.ProgramNumber).InstrumentToneMenuKey;
                using var manager = trackChunks.Merge().ManageNotes();
                var note = new Note((SevenBitNumber)number);
                var timedEvents = manager.Objects;
                note.Time = timedEvent.Time;
                timedEvents.Add(note);
            }
        }

        var trackChunk = (await 
            trackChunks.GetNoteDictionary(Song.SourceTempoMap, ProcessorConfig.Instrument.InstrumentTone,
                    ProcessorConfig.OctaveRange.LowerNote, 
                    ProcessorConfig.OctaveRange.UpperNote, 
                    (int) ProcessorConfig.Instrument.InstrumentToneMenuKey, 
                    true,
                    -ProcessorConfig.OctaveRange.LowerNote)
                .MoveNoteDictionaryToDefaultOctave(ProcessorConfig.OctaveRange)
                .ConcatNoteDictionaryToList()).ToTrackChunk();

        var playerNotesDictionary = await trackChunk.GetPlayerNoteDictionary(ProcessorConfig.PlayerCount, OctaveRange.C3toC6.LowerNote, OctaveRange.C3toC6.UpperNote);
        var concurrentPlayerTrackDictionary = new ConcurrentDictionary<long, TrackChunk>(ProcessorConfig.PlayerCount, ProcessorConfig.PlayerCount);

        Parallel.ForEach(playerNotesDictionary.Values, async (notesDictionary, _, iteration) =>
            {
                concurrentPlayerTrackDictionary[iteration] = (await notesDictionary.ConcatNoteDictionaryToList().FixChords().OffSet50Ms().FixEndSpacing()).ToTrackChunk();
                concurrentPlayerTrackDictionary[iteration].AddObjects(new List<ITimedObject>{new TimedEvent(new SequenceTrackNameEvent("tone:" + ProcessorConfig.Instrument.InstrumentTone.Name))});
            }
        );
        trackChunks = concurrentPlayerTrackDictionary.Values.ToList();
        return trackChunks;
    }
}