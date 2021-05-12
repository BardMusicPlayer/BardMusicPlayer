/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models.Structures
{
    internal class StructuresContainer
    {
        public ChatLogPointers ChatLogPointers { get; set; } = new();

        public CurrentPlayer CurrentPlayer { get; set; } = new();

        public PartyMember PartyMember { get; set; } = new();

        public PerformanceInfo PerformanceInfo { get; set; } = new();

        public World World { get; set; } = new();

        public CharacterId CharacterId { get; set; } = new();
    }
}