/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.DalamudBridge;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.Performance;
using BardMusicPlayer.Maestro.Sequencing;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Quotidian.Enums;

namespace BardMusicPlayer.Maestro
{
    public struct TitleParsingHelper
    {
        public ChatMessageChannelType channelType { get; set; }
        public string prefix { get; set; }
    }

    /// <summary>
    /// The brain of the operation;
    /// - Automatically add the found games
    /// - creates the perfomers
    /// - creates the sequencers
    /// - load songs
    /// - manages play functions
    /// </summary>
    public class Orchestrator : IDisposable
    {
        private Sequencer _sequencer { get; set; } = null;
        private CancellationTokenSource _updaterTokenSource;
        private bool LocalOchestraInitialized { get; set; } = false;
        private KeyValuePair<TitleParsingHelper, Performer> _song_Title_Parsing_Performer { get; set; } = new KeyValuePair<TitleParsingHelper, Performer>(new TitleParsingHelper{}, null);

        public int HostPid { get; set; } = 0;

        public Game HostGame { get; set; } = null;
        private List<KeyValuePair<int, Performer>> _performers { get; set; } = null;

        private Dictionary<Game, bool> _foundGames { get; set; } = null;
        private System.Timers.Timer _addPushedbackGamesTimer = null!;

        /// <summary>
        /// The constructor
        /// </summary>
        public Orchestrator()
        {
            _performers = new List<KeyValuePair<int, Performer>>();
            _foundGames = new Dictionary<Game, bool>();
            _sequencer = new Sequencer();
            _song_Title_Parsing_Performer = new KeyValuePair<TitleParsingHelper, Performer>(new TitleParsingHelper { channelType = ChatMessageChannelType.None }, null);
            BmpSeer.Instance.GameStarted += delegate (Seer.Events.GameStarted e) { Instance_OnGameStarted(e.Game); };
            BmpSeer.Instance.GameStopped += delegate (Seer.Events.GameStopped e) { Instance_OnGameStopped(e); };
            BmpSeer.Instance.EnsembleRequested += delegate (Seer.Events.EnsembleRequested e) { Instance_EnsembleRequested(e); };
            BmpSeer.Instance.EnsembleStarted += delegate (Seer.Events.EnsembleStarted e) { Instance_EnsembleStarted(e); };
            BmpSeer.Instance.EnsembleStopped += delegate (Seer.Events.EnsembleStopped e) { Instance_EnsembleStopped(e); };
            BmpSeer.Instance.InstrumentHeldChanged += delegate (Seer.Events.InstrumentHeldChanged e) { Instance_InstrumentHeldChanged(e); };

            _addPushedbackGamesTimer = new System.Timers.Timer();
            _addPushedbackGamesTimer.Interval = 2000;
            _addPushedbackGamesTimer.Enabled = false;
            _addPushedbackGamesTimer.Elapsed += CheckFoundGames;
        }

        #region public

        #region Getters
        /// <summary>
        /// Gets all games
        /// </summary>
        public IEnumerable<Game> GetAllGames()
        {
            List<Game> games =  new List<Game>();
            foreach (KeyValuePair<int, Performer> performer in _performers)
                games.Add(performer.Value.game);
            return games;
        }

        /// <summary>
        /// Gets all performers
        /// </summary>
        public IEnumerable<Performer> GetAllPerformers()
        {
            List<Performer> games = new List<Performer>();
            foreach (KeyValuePair<int, Performer> performer in _performers)
                games.Add(performer.Value);
            return games;
        }

        /// <summary>
        /// Get the host bard track number
        /// </summary>
        /// <returns>tracknumber</returns>
        public int GetHostBardTrack()
        {
            Performer perf = _performers.Where(perf => perf.Value.HostProcess).FirstOrDefault().Value;
            return perf == null ? 1 : perf.TrackNumber;
        }

        /// <summary>
        /// Get the host bard octaveshift
        /// </summary>
        /// <returns>tracknumber</returns>
        public int GetHostBardOctaveShift()
        {
            Performer perf = _performers.Where(perf => perf.Value.HostProcess).FirstOrDefault().Value;
            return perf == null ? 1 : perf.OctaveShift;
        }

        /// <summary>
        /// Get the song parsing bard
        /// </summary>
        /// <param name="p"></param>
        public KeyValuePair<TitleParsingHelper, Performer> GetSongTitleParsingBard()
        {
            return _song_Title_Parsing_Performer;
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the host game
        /// </summary>
        /// <param name="game"></param>
        public void SetHostBard(Game game)
        {
            if (game == null)
                return;

            Parallel.ForEach(_performers, perf =>
            {
                if (perf.Value.PId == game.Pid)
                {
                    perf.Value.HostProcess = true;
                    HostPid = game.Pid;
                    HostGame = game;
                }
                else
                    perf.Value.HostProcess = false;
            });
            BmpMaestro.Instance.PublishEvent(new PerformerUpdate());
        }

        /// <summary>
        /// Sets the host game
        /// </summary>
        /// <param name="p"></param>
        public void SetHostBard(Performer p)
        {
            if (p == null)
                return;

            Parallel.ForEach(_performers, perf =>
            {
                if (perf.Value.PId == p.PId)
                {
                    perf.Value.HostProcess = true;
                    HostPid = p.PId;
                    HostGame = p.game;
                }
                else
                    perf.Value.HostProcess = false;
            });
            BmpMaestro.Instance.PublishEvent(new PerformerUpdate());
        }

        /// <summary>
        /// Sets the song title parsing bard
        /// </summary>
        /// <param name="p"></param>
        public void SetSongTitleParsingBard(ChatMessageChannelType channel, string prefix, Performer p)
        {
            if (p == null)
            {
                _song_Title_Parsing_Performer = new KeyValuePair<TitleParsingHelper, Performer>(new TitleParsingHelper { }, null);
                return;
            }

            _song_Title_Parsing_Performer = new KeyValuePair<TitleParsingHelper, Performer>(new TitleParsingHelper
            {
                channelType = channel,
                prefix = prefix
            }, p);
        }

        /// <summary>
        /// sets the octaveshift for host performer (used for Ui)
        /// </summary>
        /// <param name="performer"></param>
        /// <param name="octave"></param>
        public void SetOctaveshift(Performer p, int octave)
        {
            if (p == null)
                return;
            p.OctaveShift = octave;
            BmpMaestro.Instance.PublishEvent(new OctaveShiftChangedEvent(p.game, octave, p.HostProcess));
        }

        /// <summary>
        /// sets the octaveshift for host performer (used for Ui)
        /// </summary>
        /// <param name="octave"></param>
        public void SetOctaveshiftOnHost(int octave)
        {
            foreach (var perf in _performers)
            {
                if (perf.Value.HostProcess)
                {
                    perf.Value.OctaveShift = octave;
                    BmpMaestro.Instance.PublishEvent(new OctaveShiftChangedEvent(perf.Value.game, octave, perf.Value.HostProcess));
                    return;
                }
            }
        }

        /// <summary>
        /// Seeks the song to absolute position
        /// </summary>
        /// <param name="ticks"></param>
        public void Seek(int ticks)
        {
            foreach (var perf in _performers)
                perf.Value.Sequencer.Seek(ticks);
        }

        /// <summary>
        /// Seeks the song to absolute position
        /// </summary>
        /// <param name="miliseconds"></param>
        public void Seek(double miliseconds)
        {
            foreach (var perf in _performers)
                perf.Value.Sequencer.Seek(miliseconds);
        }

        /// <summary>
        /// loads a BMPSong from the database
        /// </summary>
        /// <param name="song"></param>
        public void LoadBMPSong(BmpSong song)
        {
            if (!BmpPigeonhole.Instance.LocalOrchestra)
                LocalOchestraInitialized = false;

            _sequencer.Load(song);

            //Parse the song name if any bard should
            if (_song_Title_Parsing_Performer.Value != null)
            {
                TitleParsingHelper helper = _song_Title_Parsing_Performer.Key;

                if (BmpPigeonhole.Instance.UsePluginForInstrumentOpen && GameExtensions.IsConnected(_song_Title_Parsing_Performer.Value.game.Pid))
                {
                    //dalamud
                    string songName = $"{helper.prefix} {song.Title} {helper.prefix}";
                    GameExtensions.SendText(_song_Title_Parsing_Performer.Value.game, helper.channelType, songName);
                }
                else
                {
                    string songName = $"{helper.channelType.ChannelShortCut} {helper.prefix} {song.Title} {helper.prefix}";
                    _song_Title_Parsing_Performer.Value.SendText(songName);
                }
            }

            foreach (var perf in _performers)
            {
                perf.Value.Sequencer = _sequencer;          //use the sequence from the main sequencer
                perf.Value.Sequencer.LoadedBmpSong = song;  //set the song
            }
            InitNewPerformance();
        }

        /// <summary>
        /// sets the track for specific performer
        /// </summary>
        /// <param name="performer"></param>
        /// <param name="tracknumber"></param>
        public void SetTracknumber(Performer perf, int tracknumber)
        {
            if (perf == null)
                return;

            perf.TrackNumber = tracknumber;
        }

        /// <summary>
        /// sets the track for specific performer
        /// </summary>
        /// <param name="game"></param>
        /// <param name="tracknumber"></param>
        public void SetTracknumber(Game game, int tracknumber)
        {
            foreach (var perf in _performers)
            {
                if (perf.Value.game.Pid == game.Pid)
                {
                    perf.Value.TrackNumber = tracknumber;
                }
            }
        }

        /// <summary>
        /// sets the track for host performer (used for Ui)
        /// </summary>
        /// <param name="game"></param>
        /// <param name="tracknumber"></param>
        public void SetTracknumberOnHost(int tracknumber)
        {
            foreach (var perf in _performers)
            {
                if (perf.Value.HostProcess)
                {
                    perf.Value.TrackNumber = tracknumber;
                    BmpMaestro.Instance.PublishEvent(new TrackNumberChangedEvent(perf.Value.game, tracknumber, true));
                    return;
                }
            }
        }

        /// <summary>
        /// sets the track for all performer
        /// </summary>
        /// <param name="tracknumber"></param>
        public void SetTracknumber(int tracknumber)
        {
            foreach (var perf in _performers)
                perf.Value.TrackNumber = tracknumber;
            BmpMaestro.Instance.PublishEvent(new TrackNumberChangedEvent(null, tracknumber));
        }
        #endregion

        #region MidiInput
        /// <summary>
        /// Set the MidiInput for the first performer
        /// </summary>
        /// <param name="device"></param>
        public void OpenInputDevice(int device)
        {
            foreach (var perf in _performers)
            {
                if (perf.Value.HostProcess)
                {
                    perf.Value.Sequencer.CloseInputDevice();
                    perf.Value.Sequencer.OpenInputDevice(device);
                }
            }
        }

        /// <summary>
        /// Close the MidiInput for the first performer
        /// </summary>
        public void CloseInputDevice()
        {
            foreach (var perf in _performers)
            {
                if (perf.Value.HostProcess)
                    perf.Value.Sequencer.CloseInputDevice();
            }
        }
        #endregion

        #region Playback
        /// <summary>
        /// starts the performance
        /// </summary>
        /// <param name="delay">in ms</param>
        public void Start(int delay)
        {
            if (_performers.Count() == 0)
                return;

            //if we are a not a local orchestra
            if (!BmpPigeonhole.Instance.LocalOrchestra)
            {
                var res = _performers.Find(i => i.Value.HostProcess == true);
                res.Value.Play(true, delay);
                return;
            }

            if (delay == 0)
                delay += 100;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Parallel.ForEach(_performers, perf =>
            {
                delay = delay - (int)sw.ElapsedMilliseconds;
                if (delay < 0)
                    delay = 0;
                perf.Value.Play(true, delay);
            });
        }

        /// <summary>
        /// Pause the playback
        /// </summary>
        public void Pause()
        {
            if (_performers.Count() == 0)
                return;

            //if we are a not a local orchestra
            if (!BmpPigeonhole.Instance.LocalOrchestra)
            {
                var res = _performers.AsParallel().Where(i => i.Value.HostProcess == true);
                res.First().Value.Play(false);
                return;
            }

            foreach (var perf in _performers)
                perf.Value.Play(false);
        }

        /// <summary>
        /// Stops the playback
        /// </summary>
        public void Stop()
        {
            if (_performers.Count() == 0)
                return;

            //if we are a not a local orchestra
            if (!BmpPigeonhole.Instance.LocalOrchestra)
            {
                var res = _performers.AsParallel().Where(i => i.Value.HostProcess == true);
                res.First().Value.Stop();
                return;
            }

            foreach (var perf in _performers)
                perf.Value.Stop();
        }

        /// <summary>
        /// Equip the bard with it's instrument
        /// </summary>
        public void EquipInstruments()
        {
            try
            {
                var pList = _performers;
                Parallel.ForEach(pList, perf =>
                {
                    perf.Value.OpenInstrument();
                });
            }
            catch { }
            
        }

        /// <summary>
        /// Remove the bards instrument
        /// </summary>
        public void UnEquipInstruments()
        {
            try
            {
                var pList = _performers;
                Parallel.ForEach(pList, perf =>
                {
                    perf.Value.CloseInstrument();
                });
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// Disposing
        /// </summary>
        public void Dispose()
        {
            if (_sequencer != null)
                _sequencer.Dispose();
            // Dispose managed resources.
            if (_updaterTokenSource != null)
                _updaterTokenSource.Cancel();

            foreach (var perf in _performers)
                perf.Value.Close();

            GC.SuppressFinalize(this);
        }
        #endregion

        #region private

        #region Adding and Deleting Games from list
        /// <summary>
        /// Called if a game was found
        /// </summary>
        /// <param name="game">the found game</param>
        private void Instance_OnGameStarted(Game game)
        {
            if (BmpSeer.Instance.Games.Count == 1)
                AddPerformer(game, true);
            else
                AddPerformer(game, false);
        }

        /// <summary>
        /// Creates the performer. Is waiting till the game is ready for access
        /// </summary>
        /// <param name="game">the game</param>
        /// <param name="IsHost">is it the host game</param>
        /// <returns></returns>
        private void AddPerformer(Game game, bool IsHost)
        {
            var result = _performers.Find(kvp => kvp.Key == game.Pid);
            if (result.Key == game.Pid)
                return;

            lock (_foundGames)
            {
                if (!_foundGames.ContainsKey(game))
                    _foundGames.Add(game, IsHost);
            }
            _addPushedbackGamesTimer.Enabled = true;
        }

        /// <summary>
        /// check if the ConfigId is kown and add the performer. Triggered by the Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckFoundGames(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<Game> added = new List<Game>();
            lock (_foundGames)
            {
                foreach (var game in _foundGames)
                {
                    if (game.Key.ConfigId.Length > 0)
                    {
                        //Bard is loaded and prepared
                        Performer perf = new Performer(game.Key);
                        perf.HostProcess = game.Value;
                        perf.Sequencer = _sequencer;
                        perf.TrackNumber = 1;
                        lock (_performers)
                        {
                            //Remove the old double performer
                            var chkperf = _performers.FindAll(i => i.Value.game.PlayerName == game.Key.PlayerName);
                            if (chkperf.Count() != 0)
                            {
                                var x = chkperf.Find(i => i.Value.HomeWorld == game.Key.HomeWorld);
                                if (x.Value != null)
                                {
                                    _performers.Remove(x);
                                    x.Value.game.Dispose();
                                }
                            }
                            _performers.Add(new KeyValuePair<int, Performer>(game.Key.Pid, perf));    //Add the performer
                        }
                        BmpMaestro.Instance.PublishEvent(new PerformersChangedEvent());     //And trigger an event
                        if (game.Value)
                        {
                            HostPid = game.Key.Pid;
                            HostGame = game.Key;
                        }
                        added.Add(game.Key);
                    }
                }

                foreach (Game g in added)
                    _foundGames.Remove(g);

                if (_foundGames.Count() <= 0)
                    _addPushedbackGamesTimer.Enabled = false;
            }
        }

        /// <summary>
        /// Sets all events and starts the updater
        /// Called at every new song
        /// </summary>
        private void InitNewPerformance()
        {
            if (_updaterTokenSource != null)
                if (!_updaterTokenSource.IsCancellationRequested)
                    _updaterTokenSource.Cancel();

            //if we have a local orchestra, spread the tracknumbers across the performers
            if (BmpPigeonhole.Instance.LocalOrchestra)
            {
                Performer perfc = _performers.Where(perf => perf.Value.HostProcess).FirstOrDefault().Value;
                if (perfc != null)
                {
                    //Keep track settings for the performer
                    if (!BmpPigeonhole.Instance.EnsembleKeepTrackSetting)
                    {
                        int result = _performers.Max(p => p.Value.TrackNumber);
                        if (result != _sequencer.MaxTrack)
                            LocalOchestraInitialized = false;
                    }
                    else //reorder the performer
                    {
                        foreach (var p in _performers)
                        {
                            if (p.Value.TrackNumber > _sequencer.MaxTrack)
                                p.Value.PerformerEnabled = false;
                        }
                    }

                    //Renumber the performers if needed
                    if ((!LocalOchestraInitialized) && BmpPigeonhole.Instance.LocalOrchestra)
                    {
                        int index = 1;
                        foreach (var p in _performers)
                        {
                            if (index > _sequencer.MaxTrack)
                            {
                                p.Value.PerformerEnabled = false;
                                p.Value.TrackNumber = 0;
                            }
                            else
                            {
                                p.Value.TrackNumber = index;
                                index++;
                            }
                        }
                        LocalOchestraInitialized = true;
                    }
                }

                //if we autoequip the orchestra, just do it
                if (BmpPigeonhole.Instance.AutoEquipBards && BmpPigeonhole.Instance.LocalOrchestra)
                {
                    Parallel.ForEach(_performers, perf =>
                    {
                        if (!perf.Value.HostProcess)
                            _ = perf.Value.ReplaceInstrument();
                    });
                }
            }

            //Look up for our host bard
            Performer perf = _performers.Where(perf => perf.Value.HostProcess).FirstOrDefault().Value;
            if (perf != null)
            {
                if (BmpPigeonhole.Instance.AutoEquipBards)
                    _ = perf.ReplaceInstrument().Result;
                perf.Sequencer.PlayEnded += Sequencer_PlayEnded;
            }

            _updaterTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => Updater(_updaterTokenSource.Token), TaskCreationOptions.LongRunning);
            BmpMaestro.Instance.PublishEvent(new MaxPlayTimeEvent(_sequencer.MaxTimeAsTimeSpan, _sequencer.MaxTick));
            BmpMaestro.Instance.PublishEvent(new SongLoadedEvent(_sequencer.MaxTrack, _sequencer));
        }

        /// <summary>
        /// Called when a game was stopped
        /// </summary>
        /// <param name="g"></param>
        private void Instance_OnGameStopped(Seer.Events.GameStopped g)
        {
            RemovePerformer(g.Pid);
        }

        /// <summary>
        /// Removes a performer
        /// </summary>
        /// <param name="Pid"></param>
        private void RemovePerformer(int Pid)
        {
            var result = _performers.Find(i => i.Key == Pid);
            if (result.Value == null)
                return;

            lock (_performers)
            {
                _performers.Remove(result);
            }
            result.Value.Close();
            BmpMaestro.Instance.PublishEvent(new PerformersChangedEvent());     //trigger the event
        }
        #endregion

        #region Ensemble Events
        /// <summary>
        /// Called if a enseble request started
        /// </summary>
        /// <param name="seerEvent"></param>
        private void Instance_EnsembleRequested(Seer.Events.EnsembleRequested seerEvent)
        {
            //If we don't have alocal ochestra enabled get outa here
            if (!BmpPigeonhole.Instance.LocalOrchestra)
                return;

            _ = EnsembleAcceptAsync(seerEvent);
        }

        private async Task<int> EnsembleAcceptAsync(Seer.Events.EnsembleRequested seerEvent)
        {
            await Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay);
            var result = _performers.Find(kvp => kvp.Key == seerEvent.Game.Pid);
            if (result.Key == seerEvent.Game.Pid)
                result.Value.EnsembleAccept();

            foreach (var i in _performers)
                if (i.Value.game.Pid == seerEvent.Game.Pid)
                    i.Value.EnsembleAccept();
            return 0;
        }

        /// <summary>
        /// called when the ensemble is ready to play
        /// </summary>
        /// <param name="seerEvent"></param>
        private void Instance_EnsembleStarted(Seer.Events.EnsembleStarted seerEvent)
        {
            if (BmpPigeonhole.Instance.AutostartMethod != AutostartMethod.EnsembleMetronome)
                return;

            start(0, seerEvent.Game.Pid);
        }

        /// <summary>
        /// Stops the Ensemble if the metronome stopped
        /// </summary>
        /// <param name="seerEvent"></param>
        private void Instance_EnsembleStopped(Seer.Events.EnsembleStopped seerEvent)
        {
            if (BmpPigeonhole.Instance.AutostartMethod != AutostartMethod.EnsembleMetronome)
                return;

            if (_performers.Count() == 0)
                return;

            Task.Run(() =>
            {
                //if we are a not a local orchestra
                if (!BmpPigeonhole.Instance.LocalOrchestra)
                {
                    var res = _performers.AsParallel().Where(i => i.Value.HostProcess == true);
                    res.First().Value.Stop();
                    return;
                }

                var perf = _performers.AsParallel().Where(i => i.Value.game.Pid == seerEvent.Game.Pid);
                perf.First().Value.Stop();
            });
        }

        /// <summary>
        /// starts the performance
        /// </summary>
        /// <param name="delay">in ms</param>
        private void start(int delay, int Pid)
        {
            if (_performers.Count() == 0)
                return;

            Performer perf = _performers.Find(i => i.Key == Pid).Value;
            if (perf == null)
                return;

            if (perf.HostProcess)
                BmpMaestro.Instance.PublishEvent(new PlaybackStartedEvent());

            perf.Play(true, delay);
        }

        private void Sequencer_PlayEnded(object sender, EventArgs e)
        {
            BmpMaestro.Instance.PublishEvent(new PlaybackStoppedEvent());
        }

        /// <summary>
        /// Seer event for stopping the bards performance
        /// </summary>
        private void Instance_InstrumentHeldChanged(Seer.Events.InstrumentHeldChanged seerEvent)
        {
            Game game = seerEvent.Game;
            foreach (var perf in _performers)
            {
                if (perf.Value.game.Equals(game))
                {
                    if (game.InstrumentHeld.Equals(Instrument.None))
                        perf.Value.Stop();
                    perf.Value.PerformerEnabled = !game.InstrumentHeld.Equals(Instrument.None);
                }
            }
        }
        #endregion

        /// <summary>
        /// the updater
        /// </summary>
        /// <param name="token"></param>
        private async Task Updater(CancellationToken token)
        {
            Performer perf = _performers.Where(perf => perf.Value.HostProcess).FirstOrDefault().Value;
            while (!token.IsCancellationRequested)
            {
                //Get host performer
                if (perf == null)
                    perf = _performers.Find(perf => perf.Value.HostProcess).Value;
                else
                    BmpMaestro.Instance.PublishEvent(new CurrentPlayPositionEvent(perf.Sequencer.CurrentTimeAsTimeSpan, perf.Sequencer.CurrentTick));

                await Task.Delay(200, token).ContinueWith(tsk => { });
            }
            return;
        }
        #endregion
    }
}
