/*
 * MogLib/Common/MogLogger.cs
 *
 * Copyright (C) 2021  trotlinebeercan
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;

namespace BardMusicPlayer.Common
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