/*
 * Copyright(c) 2022 MoogleTroupe, trotlinebeercan, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer;

public partial class BmpSeer
{
    public delegate void SeerExceptionEventHandler(SeerExceptionEvent seerExceptionEvent);

    /// <summary>
    /// Called when there is an exception within the Seer frontend.
    /// </summary>
    public event SeerExceptionEventHandler SeerExceptionEvent;

    private void OnSeerExceptionEvent(SeerExceptionEvent seerExceptionEvent) =>
        SeerExceptionEvent?.Invoke(seerExceptionEvent);

    public delegate void BackendExceptionEventHandler(BackendExceptionEvent seerExceptionEvent);

    /// <summary>
    /// Called when there is an exception within a Seer Backend.
    /// </summary>
    public event BackendExceptionEventHandler BackendExceptionEvent;

    private void OnBackendExceptionEvent(BackendExceptionEvent seerExceptionEvent) =>
        BackendExceptionEvent?.Invoke(seerExceptionEvent);

    public delegate void GameExceptionEventHandler(GameExceptionEvent seerExceptionEvent);

    /// <summary>
    /// Called when there is an exception within a Seer Game.
    /// </summary>
    public event GameExceptionEventHandler GameExceptionEvent;

    private void OnGameExceptionEvent(GameExceptionEvent seerExceptionEvent) =>
        GameExceptionEvent?.Invoke(seerExceptionEvent);

    public delegate void MachinaManagerLogEventHandler(MachinaManagerLogEvent machinaManagerLogEvent);

    /// <summary>
    /// Called when there is a debug logger line from Machina internals.
    /// </summary>
    public event MachinaManagerLogEventHandler MachinaManagerLogEvent;

    private void OnMachinaManagerLogEvent(MachinaManagerLogEvent machinaManagerLogEvent) =>
        MachinaManagerLogEvent?.Invoke(machinaManagerLogEvent);

    public delegate void GameStartedHandler(GameStarted seerEvent);

    /// <summary>
    /// Called when a new ffxiv game is detected.
    /// </summary>
    public event GameStartedHandler GameStarted;

    private void OnGameStarted(GameStarted seerEvent) => GameStarted?.Invoke(seerEvent);

    public delegate void GameStoppedHandler(GameStopped seerEvent);

    /// <summary>
    /// Called when an ffxiv game disappears.
    /// </summary>
    public event GameStoppedHandler GameStopped;

    private void OnGameStopped(GameStopped seerEvent) => GameStopped?.Invoke(seerEvent);

    public delegate void ActorIdChangedHandler(ActorIdChanged seerEvent);

    /// <summary>
    /// Called when the actor id of a player changes.
    /// </summary>
    public event ActorIdChangedHandler ActorIdChanged;

    private void OnActorIdChanged(ActorIdChanged seerEvent) => ActorIdChanged?.Invoke(seerEvent);

    public delegate void ChatStatusChangedHandler(ChatStatusChanged seerEvent);

    /// <summary>
    /// Called when the chatbox is opened or closed.
    /// </summary>
    public event ChatStatusChangedHandler ChatStatusChanged;
    private void OnChatStatusChanged(ChatStatusChanged seerEvent) => ChatStatusChanged?.Invoke(seerEvent);

    public delegate void ConfigIdChangedHandler(ConfigIdChanged seerEvent);

    /// <summary>
    /// Called when the config id of a player changes.
    /// </summary>
    public event ConfigIdChangedHandler ConfigIdChanged;

    private void OnConfigIdChanged(ConfigIdChanged seerEvent) => ConfigIdChanged?.Invoke(seerEvent);

    public delegate void EnsembleRejectedHandler(EnsembleRejected seerEvent);

    /// <summary>
    /// Called when an ensemble request was rejected.
    /// </summary>
    public event EnsembleRejectedHandler EnsembleRejected;

    private void OnEnsembleRejected(EnsembleRejected seerEvent) => EnsembleRejected?.Invoke(seerEvent);

    public delegate void EnsembleRequestedHandler(EnsembleRequested seerEvent);

    /// <summary>
    /// Called when an ensemble is requested by a party leader.
    /// </summary>
    public event EnsembleRequestedHandler EnsembleRequested;

    private void OnEnsembleRequested(EnsembleRequested seerEvent) => EnsembleRequested?.Invoke(seerEvent);

    public delegate void EnsembleStartedHandler(EnsembleStarted seerEvent);

    /// <summary>
    /// Called when the metronome starts for an ensemble.
    /// </summary>
    public event EnsembleStartedHandler EnsembleStarted;

    private void OnEnsembleStarted(EnsembleStarted seerEvent) => EnsembleStarted?.Invoke(seerEvent);

    public delegate void EnsembleStoppedHandler(EnsembleStopped seerEvent);

    /// <summary>
    /// Called when the metronome starts for an ensemble.
    /// </summary>
    public event EnsembleStoppedHandler EnsembleStopped;

    private void OnEnsembleStopped(EnsembleStopped seerEvent) => EnsembleStopped?.Invoke(seerEvent);

    public delegate void HomeWorldChangedHandler(HomeWorldChanged seerEvent);

    /// <summary>
    /// Called when the home world of a player changes.
    /// </summary>
    public event HomeWorldChangedHandler HomeWorldChanged;

    private void OnHomeWorldChanged(HomeWorldChanged seerEvent) => HomeWorldChanged?.Invoke(seerEvent);

    public delegate void InstrumentHeldChangedHandler(InstrumentHeldChanged seerEvent);

    /// <summary>
    /// Called when the instrument held by the player changes.
    /// </summary>
    public event InstrumentHeldChangedHandler InstrumentHeldChanged;

    private void OnInstrumentHeldChanged(InstrumentHeldChanged seerEvent) =>
        InstrumentHeldChanged?.Invoke(seerEvent);

    public delegate void IsBardChangedHandler(IsBardChanged seerEvent);

    /// <summary>
    /// Called when the player is, or is not, a bard.
    /// </summary>
    public event IsBardChangedHandler IsBardChanged;

    private void OnIsBardChanged(IsBardChanged seerEvent) => IsBardChanged?.Invoke(seerEvent);

    public delegate void IsLoggedInChangedHandler(IsLoggedInChanged seerEvent);
    /// <summary>
    /// Called when the player is, or is not, a logged in.
    /// </summary>
    public event IsLoggedInChangedHandler IsLoggedInChanged;

    private void OnIsLoggedInChanged(IsLoggedInChanged seerEvent) => IsLoggedInChanged?.Invoke(seerEvent);

    public delegate void KeyMapChangedHandler(KeyMapChanged seerEvent);

    /// <summary>
    /// Called when the keybind configuration for a player is changed.
    /// </summary>
    public event KeyMapChangedHandler KeyMapChanged;

    private void OnKeyMapChanged(KeyMapChanged seerEvent) => KeyMapChanged?.Invoke(seerEvent);

    public delegate void NetworkPacketHandler(NetworkPacket seerEvent);

    /// <summary>
    /// Called when machina sees a network packet
    /// </summary>
    public event NetworkPacketHandler NetworkPacket;

    private void OnNetworkPacket(NetworkPacket seerEvent) => NetworkPacket?.Invoke(seerEvent);   

    public delegate void PartyMembersChangedHandler(PartyMembersChanged seerEvent);

    /// <summary>
    /// Called when the player's party changes.
    /// </summary>
    public event PartyMembersChangedHandler PartyMembersChanged;

    private void OnPartyMembersChanged(PartyMembersChanged seerEvent) => PartyMembersChanged?.Invoke(seerEvent);

    public delegate void PlayerNameChangedHandler(PlayerNameChanged seerEvent);

    /// <summary>
    /// Called when the player's name changes.
    /// </summary>
    public event PlayerNameChangedHandler PlayerNameChanged;

    private void OnPlayerNameChanged(PlayerNameChanged seerEvent) => PlayerNameChanged?.Invoke(seerEvent);

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
                    case NetworkPacket networkPacket:
                        OnNetworkPacket(networkPacket);
                        break;
                    case PartyMembersChanged partyMembersChanged:
                        OnPartyMembersChanged(partyMembersChanged);
                        break;
                    case PlayerNameChanged playerNameChanged:
                        OnPlayerNameChanged(playerNameChanged);
                        break;
                }
            }

            await Task.Delay(1, token).ContinueWith(tsk=> { }, token);
        }
    }

    internal void PublishEvent(SeerEvent seerEvent)
    {
        if (!_eventQueueOpen)
            return;

        _eventQueue.Enqueue(seerEvent);
    }

    private ConcurrentQueue<SeerEvent> _eventQueue;
    private bool _eventQueueOpen;
    private CancellationTokenSource _eventsTokenSource;

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