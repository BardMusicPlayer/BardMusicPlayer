/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class GameStarted : SeerEvent
    {
        internal GameStarted(Game game, int pid) : base(EventSource.Game)
        {
            EventType = GetType();
            Game = game;
            Pid = pid;
        }

        public int Pid { get; }

        public override bool IsValid()
        {
            return Game is not null;
        }
    }
}