/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.DalamudBridge;

public static partial class GameExtensions
{
    private static readonly SemaphoreSlim LyricSemaphoreSlim = new(1, 1);

    public static bool IsConnected(int pid)
    {
        return DalamudBridge.Instance.DalamudServer != null && DalamudBridge.Instance.DalamudServer.IsConnected(pid);
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

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
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

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
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

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
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

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
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

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendGfxLow(game.Pid, low));
    }

    /// <summary>
    /// Sets the sound
    /// </summary>
    /// <param name="game"></param>
    /// <param name="on"></param>
    /// <returns></returns>
    public static Task<bool> SetSoundOnOff(this Game game, bool on)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendSoundOnOff(game.Pid, on));
    }

    /// <summary>
    ///     starts the ensemble check
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public static Task<bool> StartEnsemble(this Game game)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendStartEnsemble(game.Pid));
    }
    
    
    /// <summary>
    /// Send the note and if it's pressed or released
    /// </summary>
    /// <param name="game"></param>
    /// <param name="noteNum"></param>
    /// <param name="pressed"></param>
    /// <returns></returns>
    /// <exception cref="DalamudBridgeException"></exception>
    public static Task<bool> SendNote(this Game game, int noteNum, bool pressed)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendNote(game.Pid, noteNum, pressed));
    }

    /// <summary>
    /// Send the program change
    /// </summary>
    /// <param name="game"></param>
    /// <param name="ProgNumber"></param>
    /// <returns></returns>
    /// <exception cref="DalamudBridgeException"></exception>
    public static Task<bool> SendProgChange(this Game game, int ProgNumber)
    {
        if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

        return Task.FromResult(DalamudBridge.Instance.DalamudServer != null &&
                               DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid) &&
                               DalamudBridge.Instance.DalamudServer.SendProgChange(game.Pid, ProgNumber));
    }
}