/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Ui.Utilities
{
    public static class LogManager
    {
        private static Controls.LogTextWriter? _LogTextWriter;

        public static bool Initialize(Controls.LogTextWriter logTextWriter)
        {
            _LogTextWriter = logTextWriter;

#if DEBUG
            SetLogLevel(BmpLog.Verbosity.Verbose);
#else
            SetLogLevel(BmpPigeonhole.Instance.DefaultLogLevel);
#endif

            BmpLog.Instance.LogEvent += WriteLog;

            BmpSeer.Instance.SeerExceptionEvent += PrintExceptionInfo;
            BmpSeer.Instance.GameExceptionEvent += PrintExceptionInfo;
            BmpSeer.Instance.BackendExceptionEvent += PrintExceptionInfo;
            BmpSeer.Instance.MachinaManagerLogEvent += PrintMachinaManagerLogEvent;
            return true;
        }

        /// <summary>
        /// Sets the log verbosity level.
        /// </summary>
        /// <param name="verbosity"></param>
        public static void SetLogLevel(BmpLog.Verbosity verbosity)
        {
            BmpPigeonhole.Instance.DefaultLogLevel = verbosity;
            BmpLog.SetMinVerbosity(verbosity);
        }

        public static bool Shutdown()
        {
            BmpSeer.Instance.SeerExceptionEvent -= PrintExceptionInfo;
            BmpSeer.Instance.GameExceptionEvent -= PrintExceptionInfo;
            BmpSeer.Instance.BackendExceptionEvent -= PrintExceptionInfo;
            BmpSeer.Instance.MachinaManagerLogEvent -= PrintMachinaManagerLogEvent;

            BmpLog.Instance.LogEvent -= WriteLog;
            return true;
        }


        private static void WriteLog(string output) => _LogTextWriter?.Write(output);
        private static void PrintExceptionInfo(SeerExceptionEvent seerExceptionEvent) => BmpLog.E(BmpLog.Source.Seer, "[" + seerExceptionEvent.EventType.Name + "] - " + seerExceptionEvent.Exception.Message);
        private static void PrintMachinaManagerLogEvent(MachinaManagerLogEvent machinaManagerLogEvent) => BmpLog.D(BmpLog.Source.Seer, "[" + machinaManagerLogEvent.EventType.Name + "] - " + machinaManagerLogEvent.Message);
    }
}
