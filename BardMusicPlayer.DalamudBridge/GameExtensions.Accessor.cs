#region

using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.DalamudBridge;

public static partial class GameExtensions
{
    private static readonly SemaphoreSlim LyricSemaphoreSlim = new(1, 1);

    public static bool IsConnected(int pid)
    {
        return DalamudBridge.Instance.DalamudServer.IsConnected(pid);
    }

    /// <summary>
    ///     Sends a lyric line via say
    /// </summary>
    /// <param name="game"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Task<bool> SendLyricLine(this Game game, string text)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendChat(game.Pid, ChatMessageChannelType.Say,
                                   text));
    }

    /// <summary>
    ///     sends a text in chat without interrupting playback
    /// </summary>
    /// <param name="game"></param>
    /// <param name="type"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Task<bool> SendText(this Game game, ChatMessageChannelType type, string text)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendChat(game.Pid, type, text));
    }

    /// <summary>
    ///     Open or close an instrument
    /// </summary>
    /// <param name="game"></param>
    /// <param name="instrumentID"></param>
    /// <returns></returns>
    public static Task<bool> OpenInstrument(this Game game, int instrumentID)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendInstrumentOpen(game.Pid, instrumentID));
    }

    /// <summary>
    ///     Accept the ens request
    /// </summary>
    /// <param name="game"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static Task<bool> AcceptEnsemble(this Game game, bool arg)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendAcceptEnsemble(game.Pid, arg));
    }

    /// <summary>
    ///     Sets the objects to low or max
    /// </summary>
    /// <param name="game"></param>
    /// <param name="low"></param>
    /// <returns></returns>
    public static Task<bool> GfxSetLow(this Game game, bool low)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendGfxLow(game.Pid, low));
    }

    /// <summary>
    ///     starts the ensemble check
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public static Task<bool> StartEnsemble(this Game game)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendStartEnsemble(game.Pid));
    }
}