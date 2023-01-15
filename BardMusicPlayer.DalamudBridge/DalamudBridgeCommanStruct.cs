#region

using BardMusicPlayer.DalamudBridge.Helper.Dalamud;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;

#endregion


namespace BardMusicPlayer.DalamudBridge;

public struct DalamudBridgeCommandStruct
{
    public MessageType messageType;
    public ChatMessageChannelType chatType;
    public Game game;
    public int IntData;
    public bool BoolData;
    public string TextData;
}