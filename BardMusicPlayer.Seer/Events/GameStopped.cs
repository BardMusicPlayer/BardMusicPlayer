/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class GameStopped : SeerEvent
    {
        /// <summary>
        /// Game will be null. This is to indicate you should refresh your known Game objects.
        /// </summary>
        /// <param name="pid">The disposed Game's Process ID</param>
        internal GameStopped(int pid) : base(EventSource.Game)
        {
            EventType = GetType();
            Pid       = pid;
        }

        public int Pid { get; }

        public override bool IsValid() => true;
    }
}