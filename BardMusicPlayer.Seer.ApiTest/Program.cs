using System;
using BardMusicPlayer.Config;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.ApiTest
{
    internal class Program
    {
        private static void Main()
        {
            BmpConfig.Initialize(AppContext.BaseDirectory + @"\Seer.ApiTest.json");

            Seer.Instance.SeerExceptionEvent += PrintExceptionInfo;
            Seer.Instance.GameExceptionEvent += PrintExceptionInfo;
            Seer.Instance.BackendExceptionEvent += PrintExceptionInfo;

            Seer.Instance.MachinaManagerLogEvent += PrintMachinaManagerLogEvent;

            Seer.Instance.GameStarted += PrintGameEventInfo;
            Seer.Instance.GameStopped += PrintGameEventInfo;

            Seer.Instance.ActorIdChanged += PrintBackendEventInfo;
            Seer.Instance.ChatStatusChanged += PrintBackendEventInfo;
            Seer.Instance.ConfigIdChanged += PrintBackendEventInfo;
            Seer.Instance.EnsembleRejected += PrintBackendEventInfo;
            Seer.Instance.EnsembleRequested += PrintBackendEventInfo;
            Seer.Instance.EnsembleStarted += PrintBackendEventInfo;
            Seer.Instance.HomeWorldChanged += PrintBackendEventInfo;
            Seer.Instance.InstrumentHeldChanged += PrintBackendEventInfo;
            Seer.Instance.KeyMapChanged += PrintBackendEventInfo;
            Seer.Instance.PartyMembersChanged += PrintBackendEventInfo;
            Seer.Instance.PlayerNameChanged += PrintBackendEventInfo;

            Seer.Instance.SetupFirewall("BardMusicPlayer.Seer.ApiTest");

            Console.WriteLine("Hit enter to start Seer");

            Console.ReadLine();

            while (true)
            {
                Seer.Instance.Start();

                Console.ReadLine();

                Seer.Instance.Stop();

                Console.WriteLine("Seer stopped. Hit enter to start it again.");

                Console.ReadLine();
            }

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
}
