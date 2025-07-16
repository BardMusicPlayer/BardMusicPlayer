/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer
{
    public sealed partial class BmpSeer
    {
        public delegate void ActorIdChangedHandler(ActorIdChanged seerEvent);

        public delegate void BackendExceptionEventHandler(BackendExceptionEvent seerExceptionEvent);

        public delegate void ChatLogHandler(ChatLog seerEvent);

        public delegate void ChatStatusChangedHandler(ChatStatusChanged seerEvent);

        public delegate void ConfigIdChangedHandler(ConfigIdChanged seerEvent);

        public delegate void EnsembleRejectedHandler(EnsembleRejected seerEvent);

        public delegate void EnsembleRequestedHandler(EnsembleRequested seerEvent);

        public delegate void EnsembleStartedHandler(EnsembleStarted seerEvent);

        public delegate void EnsembleStoppedHandler(EnsembleStopped seerEvent);

        public delegate void EnsembleStreamdataHandler(EnsembleStreamdata seerEvent);

        public delegate void GameExceptionEventHandler(GameExceptionEvent seerExceptionEvent);

        public delegate void GameStartedHandler(GameStarted seerEvent);

        public delegate void GameStoppedHandler(GameStopped seerEvent);

        public delegate void HomeWorldChangedHandler(HomeWorldChanged seerEvent);

        public delegate void InstrumentHeldChangedHandler(InstrumentHeldChanged seerEvent);

        public delegate void IsBardChangedHandler(IsBardChanged seerEvent);

        public delegate void IsLoggedInChangedHandler(IsLoggedInChanged seerEvent);

        public delegate void KeyMapChangedHandler(KeyMapChanged seerEvent);

        public delegate void MachinaManagerLogEventHandler(MachinaManagerLogEvent machinaManagerLogEvent);

        //Midibard things
        public delegate void MidibardPlaylistEventHandler(MidibardPlaylistEvent seerEvent);

        public delegate void PartyMembersChangedHandler(PartyMembersChanged seerEvent);

        public delegate void PartyLeaderChangedHandler(PartyLeaderChanged seerEvent);

        public delegate void PartyInviteHandler(PartyInvite seerEvent);

        public delegate void PlayerNameChangedHandler(PlayerNameChanged seerEvent);

        public delegate void SeerExceptionEventHandler(SeerExceptionEvent seerExceptionEvent);

        private ConcurrentQueue<SeerEvent> _eventQueue;
        private bool _eventQueueOpen;
        private CancellationTokenSource _eventsTokenSource;

        /// <summary>
        ///     Called when there is an exception within the Seer frontend.
        /// </summary>
        public event SeerExceptionEventHandler SeerExceptionEvent;

        private void OnSeerExceptionEvent(SeerExceptionEvent seerExceptionEvent)
        {
            SeerExceptionEvent?.Invoke(seerExceptionEvent);
        }

        /// <summary>
        ///     Called when there is an exception within a Seer Backend.
        /// </summary>
        public event BackendExceptionEventHandler BackendExceptionEvent;

        private void OnBackendExceptionEvent(BackendExceptionEvent seerExceptionEvent)
        {
            BackendExceptionEvent?.Invoke(seerExceptionEvent);
        }

        /// <summary>
        ///     Called when there is an exception within a Seer Game.
        /// </summary>
        public event GameExceptionEventHandler GameExceptionEvent;

        private void OnGameExceptionEvent(GameExceptionEvent seerExceptionEvent)
        {
            GameExceptionEvent?.Invoke(seerExceptionEvent);
        }

        /// <summary>
        ///     Called when there is a debug logger line from Machina internals.
        /// </summary>
        public event MachinaManagerLogEventHandler MachinaManagerLogEvent;

        private void OnMachinaManagerLogEvent(MachinaManagerLogEvent machinaManagerLogEvent)
        {
            MachinaManagerLogEvent?.Invoke(machinaManagerLogEvent);
        }

        /// <summary>
        ///     Called when a new ffxiv game is detected.
        /// </summary>
        public event GameStartedHandler GameStarted;

        private void OnGameStarted(GameStarted seerEvent)
        {
            GameStarted?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when an ffxiv game disappears.
        /// </summary>
        public event GameStoppedHandler GameStopped;

        private void OnGameStopped(GameStopped seerEvent)
        {
            GameStopped?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the actor id of a player changes.
        /// </summary>
        public event ActorIdChangedHandler ActorIdChanged;

        private void OnActorIdChanged(ActorIdChanged seerEvent)
        {
            ActorIdChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the chatbox is opened or closed.
        /// </summary>
        public event ChatStatusChangedHandler ChatStatusChanged;

        private void OnChatStatusChanged(ChatStatusChanged seerEvent)
        {
            ChatStatusChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when something happened in the chat.
        /// </summary>
        public event ChatLogHandler ChatLog;

        private void OnChatLog(ChatLog seerEvent)
        {
            ChatLog?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the config id of a player changes.
        /// </summary>
        public event ConfigIdChangedHandler ConfigIdChanged;

        private void OnConfigIdChanged(ConfigIdChanged seerEvent)
        {
            ConfigIdChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when an ensemble request was rejected.
        /// </summary>
        public event EnsembleRejectedHandler EnsembleRejected;

        private void OnEnsembleRejected(EnsembleRejected seerEvent)
        {
            EnsembleRejected?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when an ensemble is requested by a party leader.
        /// </summary>
        public event EnsembleRequestedHandler EnsembleRequested;

        private void OnEnsembleRequested(EnsembleRequested seerEvent)
        {
            EnsembleRequested?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the metronome starts for an ensemble.
        /// </summary>
        public event EnsembleStartedHandler EnsembleStarted;

        private void OnEnsembleStarted(EnsembleStarted seerEvent)
        {
            EnsembleStarted?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the metronome starts for an ensemble.
        /// </summary>
        public event EnsembleStoppedHandler EnsembleStopped;

        private void OnEnsembleStopped(EnsembleStopped seerEvent)
        {
            EnsembleStopped?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when there is data for an ensemble.
        /// </summary>
        public event EnsembleStreamdataHandler EnsembleStreamdata;

        private void OnEnsembleStreamdata(EnsembleStreamdata seerEvent)
        {
            EnsembleStreamdata?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the home world of a player changes.
        /// </summary>
        public event HomeWorldChangedHandler HomeWorldChanged;

        private void OnHomeWorldChanged(HomeWorldChanged seerEvent)
        {
            HomeWorldChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the instrument held by the player changes.
        /// </summary>
        public event InstrumentHeldChangedHandler InstrumentHeldChanged;

        private void OnInstrumentHeldChanged(InstrumentHeldChanged seerEvent)
        {
            InstrumentHeldChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the player is, or is not, a bard.
        /// </summary>
        public event IsBardChangedHandler IsBardChanged;

        private void OnIsBardChanged(IsBardChanged seerEvent)
        {
            IsBardChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the player is, or is not, a logged in.
        /// </summary>
        public event IsLoggedInChangedHandler IsLoggedInChanged;

        private void OnIsLoggedInChanged(IsLoggedInChanged seerEvent)
        {
            IsLoggedInChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the keybind configuration for a player is changed.
        /// </summary>
        public event KeyMapChangedHandler KeyMapChanged;

        private void OnKeyMapChanged(KeyMapChanged seerEvent)
        {
            KeyMapChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the player's party changes.
        /// </summary>
        public event PartyMembersChangedHandler PartyMembersChanged;

        private void OnPartyMembersChanged(PartyMembersChanged seerEvent)
        {
            PartyMembersChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the player's party leader changes.
        /// </summary>
        public event PartyLeaderChangedHandler PartyLeaderChanged;

        private void OnPartyLeaderChanged(PartyLeaderChanged seerEvent)
        {
            PartyLeaderChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when there is a party invite
        /// </summary>
        public event PartyInviteHandler PartyInvite;

        private void OnPartyInvite(PartyInvite seerEvent)
        {
            PartyInvite?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when the player's name changes.
        /// </summary>
        public event PlayerNameChangedHandler PlayerNameChanged;

        private void OnPlayerNameChanged(PlayerNameChanged seerEvent)
        {
            PlayerNameChanged?.Invoke(seerEvent);
        }

        /// <summary>
        ///     Called when something happened in the chat.
        /// </summary>
        public event MidibardPlaylistEventHandler MidibardPlaylistEvent;

        private void OnMidibardPlaylistEvent(MidibardPlaylistEvent seerEvent)
        {
            MidibardPlaylistEvent?.Invoke(seerEvent);
        }

        private async Task RunEventsHandler(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                while (_eventQueue.TryDequeue(out var seerEvent))
                {
                    if (token.IsCancellationRequested)
                        break;

                    switch (seerEvent)
                    {
                        // Exceptions
                        case MachinaManagerLogEvent machinaManagerLogEvent:
                            OnMachinaManagerLogEvent(machinaManagerLogEvent);
                            break;
                        case BackendExceptionEvent backendExceptionEvent:
                            OnBackendExceptionEvent(backendExceptionEvent);
                            break;
                        case GameExceptionEvent gameExceptionEvent:
                            OnGameExceptionEvent(gameExceptionEvent);
                            break;
                        case SeerExceptionEvent seerExceptionEvent:
                            OnSeerExceptionEvent(seerExceptionEvent);
                            break;

                        // Game events
                        case GameStarted gameStarted:
                            OnGameStarted(gameStarted);
                            break;
                        case GameStopped gameStopped:
                            OnGameStopped(gameStopped);
                            break;
                        case ActorIdChanged actorIdChanged:
                            OnActorIdChanged(actorIdChanged);
                            break;
                        case ChatStatusChanged chatStatusChanged:
                            OnChatStatusChanged(chatStatusChanged);
                            break;
                        case ChatLog chatLog:
                            OnChatLog(chatLog);
                            break;
                        case ConfigIdChanged configIdChanged:
                            OnConfigIdChanged(configIdChanged);
                            break;
                        case EnsembleRejected ensembleRejected:
                            OnEnsembleRejected(ensembleRejected);
                            break;
                        case EnsembleRequested ensembleRequested:
                            OnEnsembleRequested(ensembleRequested);
                            break;
                        case EnsembleStarted ensembleStarted:
                            OnEnsembleStarted(ensembleStarted);
                            break;
                        case EnsembleStopped ensembleStopped:
                            OnEnsembleStopped(ensembleStopped);
                            break;
                        case EnsembleStreamdata ensembleStreamdata:
                            OnEnsembleStreamdata(ensembleStreamdata);
                            break;
                        case HomeWorldChanged homeWorldChanged:
                            OnHomeWorldChanged(homeWorldChanged);
                            break;
                        case InstrumentHeldChanged instrumentHeldChanged:
                            OnInstrumentHeldChanged(instrumentHeldChanged);
                            break;
                        case IsBardChanged isBardChanged:
                            OnIsBardChanged(isBardChanged);
                            break;
                        case KeyMapChanged keyMapChanged:
                            OnKeyMapChanged(keyMapChanged);
                            break;
                        case PartyMembersChanged partyMembersChanged:
                            OnPartyMembersChanged(partyMembersChanged);
                            break;
                        case PartyLeaderChanged partyLeaderChanged:
                            OnPartyLeaderChanged(partyLeaderChanged);
                            break;
                        case PartyInvite partyInvite:
                            OnPartyInvite(partyInvite);
                            break;
                        case PlayerNameChanged playerNameChanged:
                            OnPlayerNameChanged(playerNameChanged);
                            break;
                        //Midibard things
                        case MidibardPlaylistEvent midibardPlaylistEvent:
                            OnMidibardPlaylistEvent(midibardPlaylistEvent);
                            break;
                    }
                }

                await Task.Delay(1, token).ContinueWith(static tsk => { }, token);
            }
        }

        internal void PublishEvent(SeerEvent seerEvent)
        {
            if (!_eventQueueOpen)
                return;

            _eventQueue.Enqueue(seerEvent);
        }

        private void StartEventsHandler()
        {
            _eventQueue = new ConcurrentQueue<SeerEvent>();

            _eventsTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => RunEventsHandler(_eventsTokenSource.Token), TaskCreationOptions.LongRunning);

            _eventQueueOpen = true;
        }

        private void StopEventsHandler()
        {
            _eventQueueOpen = false;
            _eventsTokenSource.Cancel();
            while (_eventQueue.TryDequeue(out _))
            {
            }
        }
    }
}