
using BardMusicPlayer.Notate.Processor.Utilities;
using BardMusicPlayer.Notate.Song.Utilities;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BardMusicPlayer.Notate.Song.Config
{
    public static class TestSong
    {
        public static async void dothings()
        {
            var timer = new Stopwatch();
            timer.Start();

            using var fileStream = File.OpenRead(@"..\..\..\Resources\the_planets.mid");

            var midiFile = fileStream.ReadAsMidiFile();

            fileStream.Dispose();

            var song = new BmpSong
            {
                Title = "test123",
                SourceTempoMap = midiFile.GetTempoMap().Clone(),
                TrackContainers = new Dictionary<long, TrackContainer>()
            };

            var trackChunkArray = midiFile.GetTrackChunks().ToArray();

            for (int i = 0; i < midiFile.GetTrackChunks().Count(); i++) song.TrackContainers[i] = new TrackContainer
            {
                SourceTrackChunk = (TrackChunk) trackChunkArray[i].Clone()
            };



            Parallel.For(0, song.TrackContainers.Count, async i =>
            {
                song.TrackContainers[i].ConfigContainers = song.TrackContainers[i].SourceTrackChunk.ReadConfigs(i, song);

                Parallel.For(0, song.TrackContainers[i].ConfigContainers.Count, async j =>
                {
                    switch (song.TrackContainers[i].ConfigContainers[j].Config)
                    {
                        case ClassicConfig classicConfig:
                            Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                              classicConfig.GetType() +
                                              " Instrument:" + classicConfig.Instrument + " OctaveRange:" +
                                              classicConfig.OctaveRange + " PlayerCount:" + classicConfig.PlayerCount +
                                              " IncludeTracks:" + string.Join(",", classicConfig.IncludedTracks));
                            song.TrackContainers[i].ConfigContainers[j].ProccesedTrackChunks =
                                await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                            break;
                        default:
                            Console.WriteLine("This should be impossible to reach.");
                            break;
                    }
                });
            });

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff")); 


            midiFile = new MidiFile(song.TrackContainers.Values.SelectMany(track => track.ConfigContainers).SelectMany(track => track.Value.ProccesedTrackChunks));
            midiFile.ReplaceTempoMap(Tools.GetMsTempoMap());
            midiFile.Write(@"..\..\..\Resources\test.mid");

        }
    }
}
