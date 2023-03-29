/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Events;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan;

internal class SharlayanReaderBackend : IReaderBackend
{
    public EventSource ReaderBackendType { get; }

    public ReaderHandler ReaderHandler { get; set; }

    public int SleepTimeInMs { get; set; }

    public SharlayanReaderBackend(int sleepTimeInMs)
    {
        ReaderBackendType = EventSource.Sharlayan;
        SleepTimeInMs     = sleepTimeInMs;
        _signaturesFound  = false;
    }

    private Reader.Reader _reader;
    private bool _signaturesFound;

    private void InitializeSharlayan()
    {
        _lastScan = new ScanItems
        {
            FirstScan          = true,
            PreviousArrayIndex = 0,
            PreviousOffset     = 0,
            ActorId            = 0,
            ConfigId           = "",
            Instrument         = Instrument.None,
            PlayerName         = "Unknown",
            IsBard             = true,
            World              = "",
            PartyMembers       = new SortedDictionary<uint, string>(),
            ChatOpen           = false
        };
        _reader ??= new Reader.Reader(new MemoryHandler(new Scanner(), ReaderHandler.Game.GameRegion));
        _reader.MemoryHandler.SetProcess(new ProcessModel { Process = ReaderHandler.Game.Process });
        _reader.MemoryHandler.SignaturesFoundEvent += SignaturesFound;
        _reader.MemoryHandler.ExceptionEvent       += ExceptionEvent;
    }

    private void DestroySharlayan()
    {
        try
        {
            if (_reader != null) _reader.MemoryHandler.SignaturesFoundEvent -= SignaturesFound;
        }
        catch
        {
            // ignored
        }

        try
        {
            if (_reader != null) _reader.MemoryHandler.ExceptionEvent -= ExceptionEvent;
        }
        catch
        {
            // ignored
        }

        try
        {
            _reader?.MemoryHandler.UnsetProcess();
        }
        catch
        {
            // ignored
        }

        try
        {
            if (_reader != null) _reader.Scanner = null;
        }
        catch
        {
            // ignored
        }

        try
        {
            if (_reader != null) _reader.MemoryHandler = null;
        }
        catch
        {
            // ignored
        }

        _reader          = null;
        _signaturesFound = false;
        _lastScan        = default;
    }

    private void SignaturesFound(object sender, SignaturesFoundEvent signaturesFoundEvent)
    {

        if (!signaturesFoundEvent.Signatures.Keys.Contains("CHATINPUT"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("CHATINPUT")));
        }

        if (!signaturesFoundEvent.Signatures.Keys.Contains("WORLD"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("WORLD")));
        }

        if (!signaturesFoundEvent.Signatures.Keys.Contains("CHARID"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("CHARID")));
        }

        if (!signaturesFoundEvent.Signatures.Keys.Contains("PERFSTATUS"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("PERFSTATUS")));
        }

        if (!signaturesFoundEvent.Signatures.Keys.Contains("PLAYERINFO"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("PLAYERINFO")));
        }

        if (!signaturesFoundEvent.Signatures.Keys.Contains("PARTYMAP"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("PARTYMAP")));
        }

        if (!signaturesFoundEvent.Signatures.Keys.Contains("PARTYCOUNT"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("PARTYCOUNT")));
        }

        if (!signaturesFoundEvent.Signatures.Keys.Contains("CHARMAP"))
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan,
                new BmpSeerSharlayanSigException("CHARMAP")));
        }

        _signaturesFound                           =  true;
        _reader.MemoryHandler.SignaturesFoundEvent -= SignaturesFound;
    }

    private void ExceptionEvent(object sender, ExceptionEvent exceptionEvent) =>
        ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan, exceptionEvent.Exception));

    private ScanItems _lastScan;

    private struct ScanItems
    {
        public bool FirstScan;
        public int PreviousArrayIndex;
        public int PreviousOffset;
        public string World;
        public Instrument Instrument;
        public string PlayerName;
        public bool IsBard;
        public bool IsLoggedIn;
        public string ConfigId;
        public uint ActorId;
        public SortedDictionary<uint, string> PartyMembers;
        public bool ChatOpen;
    }

    public async Task Loop(CancellationToken token)
    {
        InitializeSharlayan();

        while (!token.IsCancellationRequested)
        {
            try
            {
                if (!_signaturesFound || _reader.Scanner.IsScanning)
                {
                    await Task.Delay(SleepTimeInMs, token);
                    continue;
                }

                GetPlayerInfo(token);
                GetWorld(token);
                GetConfigId(token);
                GetInstrument(token);
                GetPartyMembers(token);
                GetChatInputOpen(token);

                _lastScan.FirstScan = false;
            }
            catch (Exception ex)
            {
                ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Sharlayan, ex));
            }

            await Task.Delay(SleepTimeInMs, token);
        }

        DestroySharlayan();
    }

    public void Dispose()
    {
        DestroySharlayan();
        GC.SuppressFinalize(this);
    }

    private void GetPlayerInfo(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (!_reader.CanGetPlayerInfo()) return;

        var kvp = _reader.GetCurrentPlayer();
        if (!_lastScan.FirstScan && _lastScan.ActorId.Equals(kvp.Key) &&
            _lastScan.IsBard.Equals(kvp.Value.Item2)) return;

        _lastScan.ActorId    = kvp.Key;
        _lastScan.PlayerName = kvp.Value.Item1;
        _lastScan.IsBard     = kvp.Value.Item2;
        _lastScan.IsLoggedIn = kvp.Value.Item3;
        ReaderHandler.Game.PublishEvent(new ActorIdChanged(EventSource.Sharlayan, _lastScan.ActorId));
        ReaderHandler.Game.PublishEvent(new PlayerNameChanged(EventSource.Sharlayan, _lastScan.PlayerName));
        ReaderHandler.Game.PublishEvent(new IsBardChanged(EventSource.Sharlayan, _lastScan.IsBard));
        ReaderHandler.Game.PublishEvent(new IsLoggedInChanged(EventSource.Sharlayan, _lastScan.IsLoggedIn));
    }

    private void GetConfigId(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (!_reader.CanGetCharacterId()) return;

        var configId = _reader.GetCharacterId();
        if (!_lastScan.FirstScan && _lastScan.ConfigId.Equals(configId)) return;

        _lastScan.ConfigId = configId;
        ReaderHandler.Game.PublishEvent(new ConfigIdChanged(EventSource.Sharlayan, configId));
    }

    private void GetWorld(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (!_reader.CanGetWorld()) return;

        var world = _reader.GetWorld();
        if (!_lastScan.FirstScan && _lastScan.World.Equals(world)) return;

        _lastScan.World = world;
        ReaderHandler.Game.PublishEvent(new HomeWorldChanged(EventSource.Sharlayan, world));
    }

    private void GetInstrument(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (!_reader.CanGetPerformance()) return;

        var instrument = _reader.GetPerformance();
        if (!_lastScan.FirstScan && _lastScan.Instrument.Equals(instrument)) return;

        _lastScan.Instrument = instrument;
        ReaderHandler.Game.PublishEvent(new InstrumentHeldChanged(EventSource.Sharlayan, instrument));
    }

    private void GetPartyMembers(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (!_reader.CanGetPartyMembers()) return;

        var partyResult = _reader.GetPartyMembers();
        if (!_lastScan.FirstScan && partyResult.KeysEquals(_lastScan.PartyMembers)) return;

        _lastScan.PartyMembers = partyResult;
        ReaderHandler.Game.PublishEvent(new PartyMembersChanged(EventSource.Sharlayan, _lastScan.PartyMembers));
    }

    private void GetChatInputOpen(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (!_reader.CanGetChatInput()) return;

        var result = _reader.IsChatInputOpen();
        if (!_lastScan.FirstScan && _lastScan.ChatOpen.Equals(result)) return;

        _lastScan.ChatOpen = result;
        ReaderHandler.Game.PublishEvent(new ChatStatusChanged(EventSource.Sharlayan, result));
    }
}