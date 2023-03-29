/*
 * Copyright(c) 2023 GiR-Zippo, Meowchestra
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.ObjectModel;

namespace BardMusicPlayer.Quotidian.Structs;

public readonly struct ChatMessageChannelType
{
    public static readonly ChatMessageChannelType None           = new("None",         0x0000, "");
    public static readonly ChatMessageChannelType Say            = new("Say",          0x000A, "/s");
    public static readonly ChatMessageChannelType Yell           = new("Yell",         0x001E, "/y");
    public static readonly ChatMessageChannelType Shout          = new("Shout",        0x000B, "/sh");
    public static readonly ChatMessageChannelType Party          = new("Party",        0x000E, "/p");
    public static readonly ChatMessageChannelType FreeCompany    = new("Free Company", 0x0018, "/fc");
    public static readonly IReadOnlyList<ChatMessageChannelType> All = new ReadOnlyCollection<ChatMessageChannelType>(new List<ChatMessageChannelType>
    {
        None,
        Say,
        Yell,
        Shout,
        Party,
        FreeCompany
    });

    public string Name { get; }
    public int ChannelCode { get; }
    public string ChannelShortCut { get; }

    private ChatMessageChannelType(string name, int channelCode, string channelShortCut)
    {
        Name            = name;
        ChannelCode     = channelCode;
        ChannelShortCut = channelShortCut;
    }

    public static ChatMessageChannelType ParseByChannelCode(int channelCode)
    {
        TryParseByChannelCode(channelCode, out var result);
        return result;
    }

    public static bool TryParseByChannelCode(int channelCode, out ChatMessageChannelType result)
    {
        if (All.Any(x => x.ChannelCode.Equals(channelCode)))
        {
            result = All.First(x => x.ChannelCode.Equals(channelCode));
            return true;
        }
        result = None;
        return false;
    }
}