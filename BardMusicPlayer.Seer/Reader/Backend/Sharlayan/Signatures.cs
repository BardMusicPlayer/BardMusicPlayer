/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
	internal static class Signatures
	{
        public static string CharacterMapKey { get; } = "CHARMAP";

		public static string ChatLogKey { get; } = "CHATLOG";
		
		public static string PartyCountKey { get; } = "PARTYCOUNT";

		public static string PartyMapKey { get; } = "PARTYMAP";

		public static string PlayerInformationKey { get; } = "PLAYERINFO";

		public static string PerformanceStatusKey { get; } = "PERFSTATUS";

		public static string CharacterIdKey { get; } = "CHARID";

		public static string ChatInputKey { get; } = "CHATINPUT";

		public static string WorldKey { get; } = "WORLD";

		public static IEnumerable<Signature> Resolve(MemoryHandler memoryHandler) => new APIHelper(memoryHandler).GetSignatures();
    }
}
