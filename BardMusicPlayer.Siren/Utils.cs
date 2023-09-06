/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Text.RegularExpressions;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Siren.AlphaTab.Audio.Generator;
using BardMusicPlayer.Siren.AlphaTab.Model;
using BardMusicPlayer.Transmogrify.Song;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MidiFile = BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.MidiFile;

namespace BardMusicPlayer.Siren;

internal static class Utils
{
    /// <summary>
    /// Convert the Midi for siren
    /// </summary>
    /// <param name="song"></param>
    /// <returns></returns>
    internal static async Task<(MidiFile, Dictionary<int, Dictionary<long, string>>)> GetSynthMidi(this BmpSong song)
    {
        var file = new MidiFile {Division = 600};
        var events = new AlphaSynthMidiFileHandler(file);
        events.AddTempo(0, 100);

        var trackCounter = byte.MinValue;
        var veryLast = 0L;

        var midiFile = await song.GetProcessedMidiFile();
        var trackChunks = midiFile.GetTrackChunks().ToList();
            
        var lyrics = new Dictionary<int, Dictionary<long, string>>();
        var lyricNum = 0;

        //Skip first track, it's just "All Tracks"
        foreach (var trackChunk in trackChunks.GetRange(1, trackChunks.Count-1))
        {
            var instr = Instrument.None;
            var trackOctaveShift = 0;
            using var manager = trackChunk.ManageTimedEvents();
            var trackName = trackChunk.Events.OfType<SequenceTrackNameEvent>().First().Text;
            var rex = new Regex(@"^([A-Za-z _:]+)([-+]\d)?");
            if (rex.Match(trackName) is { } match)
            {
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    instr = Instrument.Parse(match.Groups[1].Value);
                }
                if (int.TryParse(match.Groups[2].Value, out var os))
                    trackOctaveShift = os;
            }
            var instrumentMap = new Dictionary<float, KeyValuePair<NoteEvent, Instrument>>();

            foreach (var _event in manager.Objects)
            {
                var lyricsEvent = _event.Event as LyricEvent;
                var programChangeEvent = _event.Event as ProgramChangeEvent;

                if (_event.Event is NoteEvent noteEvent && _event.Event.EventType == MidiEventType.NoteOn)
                    instrumentMap.Add(_event.Time, new KeyValuePair<NoteEvent, Instrument>(noteEvent, instr));

                if (programChangeEvent != null)
                    instr = Instrument.ParseByProgramChange(programChangeEvent.ProgramNumber);

                if (lyricsEvent != null)
                {
                    if (lyrics.Count < lyricNum+1)
                        lyrics.Add(lyricNum, new Dictionary<long, string>());
                    lyrics[lyricNum].Add(_event.Time, lyricsEvent.Text);
                    lyricNum++;
                }
            }

            foreach (var note in trackChunk.GetNotes())
            {
                var instrument = instr;
                if (instrumentMap.TryGetValue(note.Time, out var test))
                {
                    if (note.NoteNumber == test.Key.NoteNumber)
                    {
                        instrument = test.Value;
                    }
                }

                var noteNum = note.NoteNumber+(12 * trackOctaveShift);
                if (noteNum < 0)   noteNum = 0;
                if (noteNum > 127) noteNum = 127;
                var dur = (int)MinimumLength(instrument, noteNum - 48, note.Length);
                var time = (int)note.Time;
                events.AddProgramChange(trackCounter, time, trackCounter,
                    (byte)instrument.MidiProgramChangeCode);
                events.AddNote(trackCounter, time, dur, (byte)noteNum, DynamicValue.FFF, trackCounter);
                if (trackCounter == byte.MaxValue)
                    trackCounter = byte.MinValue;
                else
                    trackCounter++;

                if (time + dur > veryLast) veryLast = time + dur;
            }
            instrumentMap.Clear();
        }
        trackChunks.Clear();
        events.FinishTrack(byte.MaxValue, (byte)veryLast);
        return (file, lyrics);
    }
        
    private static long MinimumLength(Instrument instrument, int note, long duration)
    {
        switch (instrument.Index)
        {
            case 1: // Harp
                switch (note)
                {
                    case <= 9:
                    case <= 19:
                        return 1338;
                    case <= 28:
                        return 1334;
                    default:
                        return 1136;
                }

            case 2: // Piano
                switch (note)
                {
                    case <= 11:
                    case <= 18:
                        return 1531;
                    case <= 25:
                        return 1530;
                    case <= 28:
                        return 1332;
                    default:
                        return 1531;
                }

            case 3: // Lute
                switch (note)
                {
                    case <= 14:
                        return 1728;
                    case <= 21:
                    case <= 28:
                        return 1727;
                    default:
                        return 1528;
                }

            case 4: // Fiddle
                switch (note)
                {
                    case <= 3:
                        return 634;
                    case <= 6:
                        return 632;
                    case <= 11:
                        return 633;
                    case <= 15:
                        return 634;
                    case <= 18:
                        return 633;
                    case <= 23:
                    case <= 30:
                        return 635;
                    default:
                        return 635;
                }

            case 5: // Flute
            case 6: // Oboe
            case 7: // Clarinet
            case 8: // Fife
            case 9: // Panpipes
                if (duration > 4500) return 4500;
                return duration < 500 ? 500 : duration;

            case 10: // Timpani
                return note switch
                {
                    <= 15 => 1193,
                    <= 23 => 1355,
                    _     => 1309
                };

            case 11: // Bongo
                return note switch
                {
                    <= 7  => 720,
                    <= 21 => 544,
                    _     => 275
                };

            case 12: // BassDrum
                return note switch
                {
                    <= 6  => 448,
                    <= 11 => 335,
                    <= 23 => 343,
                    _     => 254
                };

            case 13: // SnareDrum
                return 260;

            case 14: // Cymbal
                return 700;

            case 15: // Trumpet
            case 16: // Trombone
            case 17: // Tuba
            case 18: // Horn
            case 19: // Saxophone
            case 20: // Violin
            case 21: // Viola
            case 22: // Cello
            case 23: // DoubleBass
                return duration > 4500 ? 4500 : duration < 300 ? 300 : duration;
            case 24: // ElectricGuitarOverdriven
                return duration > 4500 ? 4500 : duration < 300 ? 300 : duration;
            case 25: // ElectricGuitarClean
            case 27: // ElectricGuitarPowerChords
                if (duration > 4500) return 4500;
                return duration < 300 ? 300 : duration;
            case 26: // ElectricGuitarMuted
                return note switch
                {
                    <= 18 => 186,
                    <= 21 => 158,
                    _     => 174
                };
            case 28: // ElectricGuitarSpecial
                return 1500;

            default: return duration;
        }
    }
}