/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan;

internal static class Signatures
{
    public static string CharacterMapKey => "CHARMAP";

    public static string PartyCountKey => "PARTYCOUNT";

    public static string PartyMapKey => "PARTYMAP";

    public static string PlayerInformationKey => "PLAYERINFO";

    public static string PerformanceStatusKey => "PERFSTATUS";

    public static string CharacterIdKey => "CHARID";

    public static string ChatInputKey => "CHATINPUT";

    public static string WorldKey => "WORLD";

    public static IEnumerable<Signature> Resolve(MemoryHandler memoryHandler) =>
        new APIHelper(memoryHandler).GetSignatures();
}