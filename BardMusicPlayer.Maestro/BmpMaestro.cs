/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Transmogrify.Song;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Maestro
{
    public partial class BmpMaestro : IDisposable
    {
        private static readonly Lazy<BmpMaestro> LazyInstance = new(() => new BmpMaestro());

        public IEnumerable<Game> Bards { get; private set; }
        public Game SelectedBard { get; set; }

        private Sequencer _sequencer;
        /// <summary>
        /// 
        /// </summary>
        public bool Started { get; private set; }

        private BmpMaestro()
        {
            Bards = BmpSeer.Instance.Games.Values;
            BmpSeer.Instance.GameStarted += e => EnsureGameExists(e.Game);
        }

        private void EnsureGameExists(Game game)
        {
            if (!Bards.Contains(game))
            {
                Bards.Append(game);
                SelectedBard ??= game;
            }
        }

        public static BmpMaestro Instance => LazyInstance.Value;


        /// <summary>
        /// Sets a new song for the sequencer
        /// </summary>
        /// <param name="bmpSong"></param>
        /// <param name="track">the tracknumber which should be played; -1 all tracks</param>
        /// <returns></returns>
        public void PlayWithLocalPerformer(BmpSong bmpSong, int track)
        {
            var index = 0;
            //create a midifile   
            var midiFile = new MidiFile();
            //add the chunks
            foreach (var data in bmpSong.TrackContainers)
            {
                //Set the channel for notes and progchanges
                using (var manager = data.Value.SourceTrackChunk.ManageNotes())
                {
                    foreach (Note note in manager.Notes)
                        note.Channel = Melanchall.DryWetMidi.Common.FourBitNumber.Parse(index.ToString());
                }
                using (var manager = data.Value.SourceTrackChunk.ManageTimedEvents())
                {
                    foreach (var e in manager.Events)
                    {
                        var programChangeEvent = e.Event as ProgramChangeEvent;
                        if (programChangeEvent == null)
                            continue;
                        programChangeEvent.Channel = Melanchall.DryWetMidi.Common.FourBitNumber.Parse(index.ToString());
                    }
                }
                midiFile.Chunks.Add(data.Value.SourceTrackChunk);

                if (data.Value.SourceTrackChunk.ManageNotes().Notes.Count() > 0)
                    index++;
            }
            //and set the tempo map
            midiFile.ReplaceTempoMap(bmpSong.SourceTempoMap);
            _sequencer = new Sequencer(SelectedBard, midiFile, track);
        }

        /// <summary>
        /// Starts the playback
        /// </summary>
        /// <returns></returns>
        public void StartLocalPerformer()
        {
            if (_sequencer != null)
                _sequencer.Start();
        }

        /// <summary>
        /// Pause the song playback
        /// </summary>
        /// <returns></returns>
        public void PauseLocalPerformer()
        {
            if (_sequencer != null)
                _sequencer.Pause();
        }

        /// <summary>
        /// Stops the song playback
        /// </summary>
        /// <returns></returns>
        public void StopLocalPerformer()
        {
            if (_sequencer != null)
                _sequencer.Stop();
        }

        /// <summary>
        /// Change the tracknumber
        /// </summary>
        /// <returns></returns>
        public void ChangeTracknumber(int track)
        {
            if (_sequencer != null)
                _sequencer.ChangeTracknumer(track);
        }

        /// <summary>
        /// Destroys the sequencer
        /// </summary>
        /// <returns></returns>
        public void DestroySongFromLocalPerformer()
        {
            if (_sequencer != null)
                _sequencer.Destroy();
        }

        public void Start()
        {
            if (Started) return;
            StartEventsHandler();
            Started = true;
        }

        public void Stop()
        {
            if (!Started) return;
            StopEventsHandler();
            Started = false;
        }


        public void SetPlaybackStart(double t)
        {
            if (_sequencer != null)
                _sequencer.SetPlaybackStart(t);
        }

        ~BmpMaestro() { Dispose(); }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}