/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
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

        public  int Pid { get; }
        public override bool IsValid() => Game is not null;
    }
}
