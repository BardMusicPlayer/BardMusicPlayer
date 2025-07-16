/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class PlayerNameChanged : SeerEvent
    {
        internal PlayerNameChanged(EventSource readerBackendType, string playerName) : base(readerBackendType)
        {
            EventType = GetType();
            PlayerName = playerName;
        }

        public string PlayerName { get; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(PlayerName);
        }
    }
}