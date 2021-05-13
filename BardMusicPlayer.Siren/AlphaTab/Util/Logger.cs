/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Util
{
    internal class Logger
    {
        public static LogLevel LogLevel { get; set; }

        static Logger()
        {
            LogLevel = LogLevel.Info;
        }

        public static void Debug(string category, string msg, object details = null)
        {
            Log(LogLevel.Debug, category, msg, details);
        }

        public static void Warning(string category, string msg, object details = null)
        {
            Log(LogLevel.Warning, category, msg, details);
        }

        public static void Info(string category, string msg, object details = null)
        {
            Log(LogLevel.Info, category, msg, details);
        }

        public static void Error(string category, string msg, object details = null)
        {
            Log(LogLevel.Error, category, msg, details);
        }

        public static void Log(LogLevel logLevel, string category, string msg, object details = null)
        {
            if (logLevel < LogLevel || LogLevel == LogLevel.None)
            {
                return;
            }

            Platform.Log(logLevel, category, msg, details);
        }
    }

    /// <summary>
    /// Defines all loglevels. 
    /// </summary>
    [JsonSerializable]
    internal enum LogLevel
    {
        /// <summary>
        /// No logging
        /// </summary>
        None = 0,

        /// <summary>
        /// Debug level (internal details are displayed).
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Info level (only important details are shown)
        /// </summary>
        Info = 2,

        /// <summary>
        /// Warning level
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Error level.
        /// </summary>
        Error = 4
    }
}
