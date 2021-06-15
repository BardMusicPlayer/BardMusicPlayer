/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Quotidian
{
    public class BmpLog
    {
        public enum Verbosity
        {
            Verbose,
            Debug,
            Info,
            Warning,
            Error
        };

        public enum Source
        {
            Coffer,
            Grunt,
            Maestro,
            Pigeonhole,
            Quotidian,
            Seer,
            Siren,
            Transmogrify,
            Ui
        }

        public static void V(Source source, string format, params object[] args) { Instance.Log(Verbosity.Verbose, source, format, args); }
        public static void D(Source source, string format, params object[] args) { Instance.Log(Verbosity.Debug, source, format, args); }
        public static void I(Source source, string format, params object[] args) { Instance.Log(Verbosity.Info, source, format, args); }
        public static void W(Source source, string format, params object[] args) { Instance.Log(Verbosity.Warning, source, format, args); }
        public static void E(Source source, string format, params object[] args) { Instance.Log(Verbosity.Error, source, format, args); }

        public static void SetMinVerbosity(Verbosity verbosity)
        { 
            Instance._minVerbosity = verbosity;
        }

        public delegate void LogEventHandler(string output);

        public event LogEventHandler LogEvent;

        public static BmpLog Instance => Arbiter.Value;
        private static readonly Lazy<BmpLog> Arbiter = new(() => new BmpLog());

        private Verbosity _minVerbosity;

        protected void Log(Verbosity verbosity, Source source, string format, params object[] args)
        {
            if (verbosity < _minVerbosity) return;
            format = "[" + verbosity + "] - [" + source + "] - " + format;
            var output = string.Format(format, args);

            LogEvent?.Invoke(output);
        }
    }
}