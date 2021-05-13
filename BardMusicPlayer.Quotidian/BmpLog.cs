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

        public static void V(string format, params object[] args) { Instance.Log(Verbosity.Verbose, format, args); }
        public static void D(string format, params object[] args) { Instance.Log(Verbosity.Debug, format, args); }
        public static void I(string format, params object[] args) { Instance.Log(Verbosity.Info, format, args); }
        public static void W(string format, params object[] args) { Instance.Log(Verbosity.Warning, format, args); }
        public static void E(string format, params object[] args) { Instance.Log(Verbosity.Error, format, args); }

        public static void SetMinVerbosity(Verbosity verbosity)
        { 
            Instance._minVerbosity = verbosity;
        }

        private static BmpLog Instance => Arbiter.Value;
        private static readonly Lazy<BmpLog> Arbiter = new(() => new BmpLog());

        private Verbosity _minVerbosity;

        protected void Log(Verbosity verbosity, string format, params object[] args)
        {
            if (verbosity < _minVerbosity) return;
            format = "[" + verbosity + "] - " + format;
            var output = string.Format(format, args);

            Console.WriteLine(output);
        }
    }
}