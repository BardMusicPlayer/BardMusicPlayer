/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public class MachinaManagerLogEvent : SeerEvent
    {
        public MachinaManagerLogEvent(string message) : base(EventSource.MachinaManager, 0, false)
        {
            EventType = GetType();
            Message   = message;
        }

        public string Message { get; }

        public override bool IsValid() => true;
    }
}