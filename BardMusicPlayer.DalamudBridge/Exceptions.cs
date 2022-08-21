using BardMusicPlayer.Quotidian;

namespace BardMusicPlayer.DalamudBridge
{
    public class DalamudBridgeException : BmpException
    {
        internal DalamudBridgeException() : base()
        {
        }
        internal DalamudBridgeException(string message) : base(message)
        {
        }
    }
}
