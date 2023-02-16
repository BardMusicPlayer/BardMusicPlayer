#region

using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.DalamudBridge;

public static partial class GameExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public static bool IsDalamudHooked(this Game game)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("Grunt not started.");

        return DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid);
    }
}