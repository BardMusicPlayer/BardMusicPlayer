/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Processor.Utilities;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Notate.Processor
{
    internal class AutoToneProcessor : BaseProcessor
    {
        public AutoToneProcessorConfig ProcessorConfig { get; set; }

        internal AutoToneProcessor(AutoToneProcessorConfig processorConfig, BmpSong song) : base(song)
        {
            ProcessorConfig = processorConfig;
        }

        public override async Task<List<TrackChunk>> Process()
        {
            var trackChunks = new List<TrackChunk> { Song.TrackContainers[ProcessorConfig.Track].SourceTrackChunk }.Concat(ProcessorConfig.IncludedTracks.Select(track => Song.TrackContainers[track].SourceTrackChunk)).ToList();

            var instrumentGroup1 = trackChunks.GetNoteDictionary(Song.SourceTempoMap, ProcessorConfig.AutoToneInstrumentGroup.InstrumentTone,
                ProcessorConfig.AutoToneOctaveRange.LowerNote,
                ProcessorConfig.AutoToneOctaveRange.LowerNote + ProcessorConfig.AutoToneInstrumentGroup.Size1 * 12 - 1,
                (int) ProcessorConfig.AutoToneInstrumentGroup.Instrument1.InstrumentToneMenuKey,
                false,
                -OctaveRange.Parse(ProcessorConfig.AutoToneOctaveRange.Index).LowerNote)
                .MoveNoteDictionaryToDefaultOctave(OctaveRange.Parse(ProcessorConfig.AutoToneOctaveRange.Index))
                .ConcatNoteDictionaryToList();

            var instrumentGroup2 = trackChunks.GetNoteDictionary(Song.SourceTempoMap, ProcessorConfig.AutoToneInstrumentGroup.InstrumentTone,
                ProcessorConfig.AutoToneOctaveRange.LowerNote + ProcessorConfig.AutoToneInstrumentGroup.Size1 * 12,
                ProcessorConfig.AutoToneOctaveRange.LowerNote + ProcessorConfig.AutoToneInstrumentGroup.Size1 * 12 + ProcessorConfig.AutoToneInstrumentGroup.Size2 * 12 - 1,
                (int) ProcessorConfig.AutoToneInstrumentGroup.Instrument2.InstrumentToneMenuKey,
                false,
                -OctaveRange.Parse(ProcessorConfig.AutoToneOctaveRange.Index + 1).LowerNote)
                .MoveNoteDictionaryToDefaultOctave(OctaveRange.Parse(ProcessorConfig.AutoToneOctaveRange.Index + 1))
                .ConcatNoteDictionaryToList();

            var instrumentGroup3 = trackChunks.GetNoteDictionary(Song.SourceTempoMap, ProcessorConfig.AutoToneInstrumentGroup.InstrumentTone,
                ProcessorConfig.AutoToneOctaveRange.LowerNote + ProcessorConfig.AutoToneInstrumentGroup.Size1 * 12 + ProcessorConfig.AutoToneInstrumentGroup.Size2 * 12,
                ProcessorConfig.AutoToneOctaveRange.LowerNote + ProcessorConfig.AutoToneInstrumentGroup.Size1 * 12 + ProcessorConfig.AutoToneInstrumentGroup.Size2 * 12 + ProcessorConfig.AutoToneInstrumentGroup.Size3 * 12,
                (int) ProcessorConfig.AutoToneInstrumentGroup.Instrument3.InstrumentToneMenuKey,
                false,
                -OctaveRange.Parse(ProcessorConfig.AutoToneOctaveRange.Index + 2).LowerNote)
                .MoveNoteDictionaryToDefaultOctave(OctaveRange.Parse(ProcessorConfig.AutoToneOctaveRange.Index + 2))
                .ConcatNoteDictionaryToList();

            await Task.WhenAll(instrumentGroup1, instrumentGroup2, instrumentGroup3);

            var notes = new List<Note>();

            if (!ProcessorConfig.AutoToneInstrumentGroup.Instrument1.Equals(Instrument.None)) notes.AddRange(instrumentGroup1.Result);
            if (!ProcessorConfig.AutoToneInstrumentGroup.Instrument2.Equals(Instrument.None)) notes.AddRange(instrumentGroup2.Result);
            if (!ProcessorConfig.AutoToneInstrumentGroup.Instrument3.Equals(Instrument.None)) notes.AddRange(instrumentGroup3.Result);
            
            var trackChunk = TimedObjectUtilities.ToTrackChunk(notes);

            var playerNotesDictionary = await trackChunk.GetPlayerNoteDictionary(ProcessorConfig.PlayerCount, OctaveRange.C3toC6.LowerNote, OctaveRange.C3toC6.UpperNote);

            var concurrentPlayerTrackDictionary = new ConcurrentDictionary<long, TrackChunk>(ProcessorConfig.PlayerCount, ProcessorConfig.PlayerCount);

            Parallel.ForEach(playerNotesDictionary.Values, async (notesDictionary, _, iteration) =>
                {
                    concurrentPlayerTrackDictionary[iteration] = TimedObjectUtilities.ToTrackChunk(await notesDictionary.ConcatNoteDictionaryToList().FixChords().OffSet50Ms().FixEndSpacing());
                    concurrentPlayerTrackDictionary[iteration].AddObjects(new List<ITimedObject>{new TimedEvent(new SequenceTrackNameEvent("tone:" + ProcessorConfig.AutoToneInstrumentGroup.InstrumentTone.Name))});
                }
            );

            trackChunks = concurrentPlayerTrackDictionary.Values.ToList();

            return trackChunks;
        }
    }
}
