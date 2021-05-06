/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Common;
using BardMusicPlayer.Config.JsonSettings.Autosave;

namespace BardMusicPlayer.Config
{
    public class BmpConfig : JsonSettings.JsonSettings
    {
        private static BmpConfig _instance;
        public BmpConfig() { }
        public BmpConfig(string fileName) : base(fileName) { }

        /// <summary>
        /// This configuration's filename
        /// </summary>
        public override string FileName { get; set; }

        /// <summary>
        /// Initializes the config file
        /// </summary>
        /// <param name="filename">full path to the json config file</param>
        public static void Initialize(string filename)
        {
            if (_instance != null) return;
            Load<BmpConfig>(filename).EnableAutosave();
        }

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


    }
}
