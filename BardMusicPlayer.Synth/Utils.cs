using System.Linq;
using BardMusicPlayer.Common;
using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Objects;
using BardMusicPlayer.Synth.AlphaTab.Audio.Generator;
using BardMusicPlayer.Synth.AlphaTab.Audio.Synth.Midi;
using BardMusicPlayer.Synth.AlphaTab.Model;

namespace BardMusicPlayer.Synth
{
    internal static class Utils
    {
        internal static MidiFile GetSynthMidi(this MMSong mmSong)
        {
            var file = new MidiFile {Division = 600};
            
            var events = new AlphaSynthMidiFileHandler(file);
            events.AddTempo(0, 100);

            var trackCounter = 0;

            var veryLast = 0L;
            foreach (var bard in mmSong.bards)
            {
                var failure = false;
                long lastTime = 0;
                var lastNote = 254;
                var currentInstrument = bard.instruments[NotateConfig.NotateGroup.VST.VST0];
                
                foreach (var noteEvent in bard.sequence.Where(_ => !failure))
                { 
                    var key = noteEvent.Key;
                    var value = noteEvent.Value;
                    if (lastNote == 254)
                    {
                        switch (value)
                        {
                            case > 120: // VST Switch
                                currentInstrument = bard.instruments[(NotateConfig.NotateGroup.VST) value];
                                break;
                            case <= 60 and >= 24 when key * 25 % 100 == 50 || key * 25 % 100 == 0:
                                lastNote = value;
                                if(!OctaveRange.C3toC6.TryShiftNoteToOctave(OctaveRange.C1toC4, ref lastNote)) failure = true;
                                lastTime = key * 25; 
                                break; 
                            default:
                                failure = true;
                                break;
                        } 
                    }  
                    else
                    {
                        if (value == 254)
                        {
                            var dur = (int) MinimumLength(currentInstrument, lastNote-48, key * 25 - lastTime);
                            var time = (int) (  lastTime);
                            events.AddProgramChange(trackCounter, time, (byte) trackCounter, (byte) currentInstrument.MidiProgramChangeCode);
                            events.AddNote(trackCounter, time, dur,(byte) lastNote, DynamicValue.FFF, (byte) trackCounter);

                            if (time + dur > veryLast) veryLast = time + dur;

                            trackCounter++;
                            if (trackCounter == byte.MaxValue) trackCounter = 0;

                            lastNote = 254; 
                            lastTime = key * 25;
                        }
                        else failure = true;
                    }
                }
                if (failure) throw new BmpException("Error loading MMSong into Synth");
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
                    return duration < 100 ? 100 : duration;

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
                    return duration < 100 ? 100 : duration;

                default: return duration;
            }
        }
    }
}
