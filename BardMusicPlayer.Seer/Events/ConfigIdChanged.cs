/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

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

        public override bool IsValid() => !string.IsNullOrEmpty(ConfigId) && ConfigId.StartsWith("FFXIV_CHR") && ConfigId.Length == 25;
    }
}
