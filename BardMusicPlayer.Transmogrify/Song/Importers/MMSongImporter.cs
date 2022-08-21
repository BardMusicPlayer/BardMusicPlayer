using System.IO;
using System.IO.Compression;
using System.Linq;
using BardMusicPlayer.Quotidian.Structs;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;

namespace BardMusicPlayer.Transmogrify.Song.Importers
{
    public static class MMSongImporter
    {
        /// <summary>
        /// Opens and process a mmsong file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MidiFile OpenMMSongFile(string path)
        {
            if (!File.Exists(path)) throw new BmpTransmogrifyException("File " + path + " does not exist!");

            MMSongContainer songContainer = null;

            FileInfo fileToDecompress = new FileInfo(path);
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        decompressionStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        var data = "";
                        using (var reader = new StreamReader(memoryStream, System.Text.Encoding.ASCII))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                data += line;
                            }
                        }
                        memoryStream.Close();
                        decompressionStream.Close();
                        songContainer = JsonConvert.DeserializeObject<MMSongContainer>(data);
                    }
                }
            }

            MidiFile midiFile = new MidiFile();
            foreach (MMSong msong in songContainer.songs)
            {
                if (msong.bards.Count() == 0)
                    continue;
                else
                {
                    foreach (var bard in msong.bards)
                    {
                        var thisTrack = new TrackChunk(new SequenceTrackNameEvent(Instrument.Parse(bard.instrument).Name));
                        using (var manager = new TimedEventsManager(thisTrack.Events))
                        {
                            TimedEventsCollection timedEvents = manager.Events;
                            int last = 0;
                            foreach (var note in bard.sequence)
                            {
                                if (note.Value == 254)
                                {
                                    var pitched = last + 24;
                                    timedEvents.Add(new TimedEvent(new NoteOffEvent((SevenBitNumber)pitched, (SevenBitNumber)127), note.Key));
                                }
                                else
                                {
                                    var pitched = (SevenBitNumber)note.Value + 24;
                                    timedEvents.Add(new TimedEvent(new NoteOnEvent((SevenBitNumber)pitched, (SevenBitNumber)127), note.Key));
                                    last = note.Value;
                                }
                            }
                        }
                        midiFile.Chunks.Add(thisTrack);
                    };

                    foreach(var lyrics in msong.lyrics)
                    {
                        var thisTrack = new TrackChunk(new SequenceTrackNameEvent("Lyrics: " + lyrics.description));
                        using (var manager = new TimedEventsManager(thisTrack.Events))
                        {
                            TimedEventsCollection timedEvents = manager.Events;
                            foreach (var seqData in lyrics.sequence)
                            {
                                var f = lyrics.lines[seqData.Value];
                                timedEvents.Add(new TimedEvent(new LyricEvent(lyrics.lines[seqData.Value]), seqData.Key));
                            }
                        }
                        midiFile.Chunks.Add(thisTrack);
                    }

                    break; //Only the first song for now
                }
            }
            midiFile.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(25)));
            return midiFile;
        }
    }
}
