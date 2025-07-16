/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class MachinaManagerLogEvent : SeerEvent
    {
        public MachinaManagerLogEvent(string message) : base(EventSource.MachinaManager)
        {
            EventType = GetType();
            Message = message;
        }

        public string Message { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}