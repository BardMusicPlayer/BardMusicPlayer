/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayerApi/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.ApiTest;

internal class Program
{
    private static void Main()
    {
        BmpPigeonhole.Initialize(AppContext.BaseDirectory + @"\Seer.ApiTest.json");

        BmpSeer.Instance.SeerExceptionEvent    += PrintExceptionInfo;
        BmpSeer.Instance.GameExceptionEvent    += PrintExceptionInfo;
        BmpSeer.Instance.BackendExceptionEvent += PrintExceptionInfo;

        BmpSeer.Instance.MachinaManagerLogEvent += PrintMachinaManagerLogEvent;

        BmpSeer.Instance.GameStarted += PrintGameEventInfo;
        BmpSeer.Instance.GameStopped += PrintGameEventInfo;

        BmpSeer.Instance.ActorIdChanged        += PrintBackendEventInfo;
        BmpSeer.Instance.ChatStatusChanged     += PrintBackendEventInfo;
        BmpSeer.Instance.ConfigIdChanged       += PrintBackendEventInfo;
        BmpSeer.Instance.EnsembleRejected      += PrintBackendEventInfo;
        BmpSeer.Instance.EnsembleRequested     += PrintBackendEventInfo;
        BmpSeer.Instance.EnsembleStarted       += PrintBackendEventInfo;
        BmpSeer.Instance.HomeWorldChanged      += PrintBackendEventInfo;
        BmpSeer.Instance.InstrumentHeldChanged += PrintBackendEventInfo;
        BmpSeer.Instance.IsBardChanged         += PrintBackendEventInfo;
        BmpSeer.Instance.KeyMapChanged         += PrintBackendEventInfo;
        BmpSeer.Instance.PartyMembersChanged   += PrintBackendEventInfo;
        BmpSeer.Instance.PlayerNameChanged     += PrintBackendEventInfo;
        BmpSeer.Instance.NetworkPacket         += PrintBackendEventInfo;

        BmpSeer.Instance.SetupFirewall("BardMusicPlayer.Seer.ApiTest");

        BmpSeer.Instance.Start();

        Console.ReadLine();
    }

    private static void PrintExceptionInfo(SeerExceptionEvent seerExceptionEvent)
    {
        Console.WriteLine("Exception: " + seerExceptionEvent.EventType + " " + seerExceptionEvent.Exception.Message);
    }

    private static void PrintMachinaManagerLogEvent(MachinaManagerLogEvent machinaManagerLogEvent)
    {
        Console.WriteLine("MachinaManager: " + machinaManagerLogEvent.Message);
    }

    private static void PrintGameEventInfo(SeerEvent seerEvent)
    {
        Console.WriteLine("GameEvent: " + seerEvent.EventType);
    }

    private static void PrintBackendEventInfo(SeerEvent seerEvent)
    {
        Console.WriteLine(seerEvent.EventSource + ": Pid:" + seerEvent.Game.Pid + " Event Type:" + seerEvent.EventType);
    }
}