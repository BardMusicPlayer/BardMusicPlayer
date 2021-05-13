/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Common;
using BardMusicPlayer.Config.JsonSettings.Autosave;

namespace BardMusicPlayer.Config
{
    public class BmpConfig : JsonSettings.JsonSettings
    {
        private static BmpConfig _instance;

        /// <summary>
        /// Initializes the config file
        /// </summary>
        /// <param name="filename">full path to the json config file</param>
        public static void Initialize(string filename)
        {
            if (Initialized) return;
            _instance = Load<BmpConfig>(filename).EnableAutosave();
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Initialized => _instance != null;

        /// <summary>
        /// Gets this configuration instance
        /// </summary>
        public static BmpConfig Instance => _instance ?? throw new BmpException("This configuration must be initialized first.");

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
    }
}
