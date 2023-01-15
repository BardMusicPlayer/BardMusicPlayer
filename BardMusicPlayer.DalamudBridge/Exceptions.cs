#region

using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.DalamudBridge;

public sealed class DalamudBridgeException : BmpException
{
    internal DalamudBridgeException()
    {
    }

    internal DalamudBridgeException(string message) : base(message)
    {
    }
}