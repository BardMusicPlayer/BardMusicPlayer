using System;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Synth.AlphaTab.Audio.Generator;
using BardMusicPlayer.Synth.AlphaTab.Model;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MidiFile = BardMusicPlayer.Synth.AlphaTab.Audio.Synth.Midi.MidiFile;

namespace BardMusicPlayer.Synth
{
    internal static class Utils
    {
        internal static async Task<MidiFile> GetSynthMidi(this BmpSong song)
        {
            var file = new MidiFile {Division = 600};
            var events = new AlphaSynthMidiFileHandler(file);
            events.AddTempo(0, 100);

            var trackCounter = 0;
            var veryLast = 0L;

            var midiFile = await song.GetProcessedMidiFile();

            foreach (var trackChunk in midiFile.GetTrackChunks())
            {
                var options = trackChunk.Events.OfType<SequenceTrackNameEvent>().First().Text.Split(':');
                if (options[0].Equals("lyric"))
                {
                    // TODO
                }
                else if (options[0].Equals("tone"))
                {
                    var tone = InstrumentTone.Parse(options[1]);
                    foreach (var note in trackChunk.GetNotes())
                    {
                        var instrument = tone.GetInstrumentFromChannel(note.Channel);
                        var noteNum = note.NoteNumber;
                        var dur = (int) MinimumLength(instrument, noteNum-48, note.Length);
                        var time = (int) note.Time;
                        events.AddProgramChange(trackCounter, time, (byte) trackCounter, (byte) instrument.MidiProgramChangeCode);
                        events.AddNote(trackCounter, time, dur,noteNum, DynamicValue.FFF, (byte) trackCounter);
                        trackCounter++;
                        if (trackCounter == byte.MaxValue) trackCounter = 0;
                        if (time + dur > veryLast) veryLast = time + dur;
                    }
                    
                }
            }
            events.FinishTrack(byte.MaxValue, (byte) veryLast);
            return file;
        }
        
        private static long MinimumLength(Instrument instrument, int note, long duration)
        {
            switch (instrument.Index)
            {
                case 1: // Harp
                    if (note <= 9) return 1338;
                    else if (note <= 19) return 1338;
                    else if (note <= 28) return 1334;
                    else return 1136;

                case 2: // Piano
                    if (note <= 11) return 1531;
                    else if (note <= 18) return 1531;
                    else if (note <= 25) return 1530;
                    else if (note <= 28) return 1332;
                    else return 1531;

                case 3: // Lute
                    if (note <= 14) return 1728;
                    else if (note <= 21) return 1727;
                    else if (note <= 28) return 1727;
                    else return 1528;

                case 4: // Fiddle
                    if (note <= 3) return 634;
                    else if (note <= 6) return 632;
                    else if (note <= 11) return 633;
                    else if (note <= 15) return 634;
                    else if (note <= 18) return 633;
                    else if (note <= 23) return 635;
                    else if (note <= 30) return 635;
                    else return 635;

                case 5: // Flute
                case 6: // Oboe
                case 7: // Clarinet
                case 8: // Fife
                case 9: // Panpipes
                    if (duration > 4500) return 4500;
                    return duration < 500 ? 500 : duration;

                case 10: // Timpani
                    if (note <= 15) return 1193;
                    else if (note <= 23) return 1355;
                    else return 1309;

                case 11: // Bongo
                    if (note <= 7) return 720;
                    else if (note <= 21) return 544;
                    else return 275;

                case 12: // BassDrum
                    if (note <= 6) return 448;
                    else if (note <= 11) return 335;
                    else if (note <= 23) return 343;
                    else return 254;

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
                    if (duration > 4500) return 4500;
                    return duration < 300 ? 300 : duration;

                default: return duration;
            }
        }
    }
}
