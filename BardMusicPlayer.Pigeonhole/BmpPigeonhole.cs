/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Pigeonhole.JsonSettings.Autosave;
using BardMusicPlayer.Quotidian;

namespace BardMusicPlayer.Pigeonhole
{
    public class BmpPigeonhole : JsonSettings.JsonSettings
    {
        private static BmpPigeonhole _instance;

        /// <summary>
        /// Initializes the pigeonhole file
        /// </summary>
        /// <param name="filename">full path to the json pigeonhole file</param>
        public static void Initialize(string filename)
        {
            if (Initialized) return;

            _instance = Load<BmpPigeonhole>(filename).EnableAutosave();
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Initialized => _instance != null;

        /// <summary>
        /// Gets this pigeonhole instance
        /// </summary>
        public static BmpPigeonhole Instance =>
            _instance ?? throw new BmpException("This pigeonhole must be initialized first.");

        /// <summary>
        /// Enable the 16 voice limit in Synthesizer
        /// </summary>
        public virtual bool EnableSynthVoiceLimiter { get; set; } = true;

        /// <summary>
        /// milliseconds between game process scans / seer scanner startups.
        /// </summary>
        public virtual int SeerGameScanCooldown { get; set; } = 20;

        /// <summary>
        /// Contains the last path of an opened midi file
        /// </summary>
        public virtual string LastOpenedMidiPath { get; set; } = "";

        /// <summary>
        /// Contains the delay used for note pressing. This should be no less then 1 and no greater then 25.
        /// </summary>
        public virtual int NoteKeyDelay { get; set; } = 1;

        /// <summary>
        /// Contains the delay used for tone pressing. This should be no less then 1 and no greater then 25.
        /// </summary>
        public virtual int ToneKeyDelay { get; set; } = 3;

        /// <summary>
        /// Defaults to log level Info
        /// </summary>
        public virtual BmpLog.Verbosity DefaultLogLevel { get; set; } = BmpLog.Verbosity.Info;
    }
}