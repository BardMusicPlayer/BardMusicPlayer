/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ConfigIdChanged : SeerEvent
    {
        internal ConfigIdChanged(EventSource readerBackendType, string configId) : base(readerBackendType)
        {
            EventType = GetType();
            ConfigId = configId;
        }

        public string ConfigId { get; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(ConfigId) && ConfigId.StartsWith("FFXIV_CHR", StringComparison.Ordinal) &&
                   ConfigId.Length == 25;
        }
    }
}