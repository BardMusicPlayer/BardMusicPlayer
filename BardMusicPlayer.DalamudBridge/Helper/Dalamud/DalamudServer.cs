/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

#nullable enable

#region

using System.Collections.Concurrent;
using System.Diagnostics;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;
using H.Formatters;
using H.Pipes;
using H.Pipes.AccessControl;
using H.Pipes.Args;

#endregion

namespace BardMusicPlayer.DalamudBridge.Helper.Dalamud;

public sealed class PayloadMessage
{
    public MessageType MsgType { get; set; } = MessageType.None;
    public int MsgChannel { get; set; }
    public string Message { get; set; } = "";
}

internal sealed class DalamudServer : IDisposable
{
    private readonly ConcurrentDictionary<int, string> _clients;
    private readonly PipeServer<PayloadMessage> _pipe;

    private readonly Version minVersion = new Version("0.0.1.8");

    /// <summary>
    /// </summary>
    internal DalamudServer()
    {
        _clients                 =  new ConcurrentDictionary<int, string>();
        _pipe                    =  new PipeServer<PayloadMessage>("Hypnotoad", new NewtonsoftJsonFormatter());
        _pipe.ClientConnected    += OnConnected;
        _pipe.ClientDisconnected += OnDisconnected;
        _pipe.MessageReceived    += OnMessage;
#pragma warning disable CA1416
        _pipe.AllowUsersReadWrite();
#pragma warning restore CA1416
        Start();
    }

    /// <summary>
    ///     Disposing
    /// </summary>
    public void Dispose()
    {
        try
        {
            Stop();
            _pipe.MessageReceived    -= OnMessage;
            _pipe.ClientConnected    -= OnDisconnected;
            _pipe.ClientDisconnected -= OnConnected;
            _pipe.DisposeAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Dalamud error: {ex.Message}");
        }

        _clients.Clear();
    }

    internal async void Start()
    {
        if (_pipe.IsStarted) return;

        await _pipe.StartAsync();
    }

    internal async void Stop()
    {
        if (!_pipe.IsStarted) return;

        await _pipe.StopAsync();
    }

    /// <summary>
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    internal bool IsConnected(int pid)
    {
        return _clients.ContainsKey(pid) &&
               _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected) is not null;
    }

    /// <summary>
    ///     send a text to the toad, to type during playback
    /// </summary>
    /// <param name="pid">proc Id</param>
    /// <param name="chanType">channel type</param>
    /// <param name="text">the message</param>
    /// <returns></returns>
    internal bool SendChat(int pid, ChatMessageChannelType chanType, string text)
    {
        if (!IsConnected(pid))
            return false;

        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(
            new PayloadMessage
            {
                MsgType    = MessageType.Chat,
                MsgChannel = chanType.ChannelCode,
                Message    = text
            });
        return true;
    }

    /// <summary>
    ///     Send instrument open action to the toad
    /// </summary>
    /// <param name="pid">proc Id</param>
    /// <param name="instrumentID">XIV instrument number</param>
    /// <returns></returns>
    internal bool SendInstrumentOpen(int pid, int instrumentID)
    {
        if (!IsConnected(pid))
            return false;

        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(
            new PayloadMessage
            {
                MsgType = MessageType.Instrument,
                Message = instrumentID.ToString()
            });
        return true;
    }

    /// <summary>
    ///     accept ensemble request action
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal bool SendAcceptEnsemble(int pid, bool arg)
    {
        if (!IsConnected(pid))
            return false;

        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(
            new PayloadMessage
            {
                MsgType = MessageType.AcceptReply,
                Message = ""
            });
        return true;
    }

    /// <summary>
    ///     switch gfx to low and back
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal bool SendGfxLow(int pid, bool arg)
    {
        if (!IsConnected(pid))
            return false;

        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(
            new PayloadMessage
            {
                MsgType = MessageType.SetGfx,
                Message = arg ? "1" : "0"
            });
        return true;
    }

    /// <summary>
    /// Send ensemble start
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    internal bool SendStartEnsemble(int pid)
    {
        if (!IsConnected(pid))
            return false;

        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(
            new PayloadMessage
            {
                MsgType = MessageType.StartEnsemble,
                Message = ""
            });
        return true;
    }

    /// <summary>
    /// Send the note and if it's pressed or released
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="note"></param>
    /// <param name="pressed"></param>
    /// <returns></returns>
    internal bool SendNote(int pid, int note, bool pressed)
    {
        if (!IsConnected(pid))
            return false;

        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(new PayloadMessage
        {
            MsgType = pressed ? MessageType.NoteOn : MessageType.NoteOff,
            Message = note.ToString()
        });
        return true;
    }

    /// <summary>
    /// Send the progchange
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="ProgNumber"></param>
    /// <returns></returns>
    internal bool SendProgChange(int pid, int ProgNumber)
    {
        if (!IsConnected(pid))
            return false;

        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(new PayloadMessage
        {
            MsgType = MessageType.ProgramChange,
            Message = ProgNumber.ToString()
        });
        return true;
    }

    /// <summary>
    ///     If message from client rec
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMessage(object? sender, ConnectionMessageEventArgs<PayloadMessage?> e)
    {
        var inMsg = e.Message;
        if (inMsg == null)
            return;

        switch (inMsg.MsgType)
        {
            case MessageType.Handshake:
            {
                var t = Convert.ToInt32(inMsg.Message);
                _clients.TryAdd(t, e.Connection.PipeName);
                Debug.WriteLine($"Dalamud client Id {e.Connection.PipeName} {t} connected");
                break;
            }
            case MessageType.Version:
                try
                {
                    var t = inMsg.Message;
                    var pid = Convert.ToInt32(t.Split(':')[0]);
                    var version = new Version(t.Split(':')[1]);
                    if (version < minVersion)
                    {
                        _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(
                            new PayloadMessage
                            {
                                MsgType = MessageType.Version,
                                Message = minVersion.ToString()
                            });
                    }
                }
                catch
                {
                    // ignored
                }

                break;
            case MessageType.SetGfx:
                try
                {
                    var t = inMsg.Message;
                    var pid = Convert.ToInt32(t.Split(':')[0]);
                    var lowsettings = Convert.ToBoolean(t.Split(':')[1]);
                    if (BmpSeer.Instance.Games.ContainsKey(pid))
                        BmpSeer.Instance.Games[pid].GfxSettingsLow = lowsettings;
                }
                catch
                {
                    // ignored
                }

                break;
        }
    }

    /// <summary>
    ///     If client disconnected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDisconnected(object? sender, ConnectionEventArgs<PayloadMessage> e)
    {
        if (_clients.Values.Contains(e.Connection.PipeName))
            _clients.TryRemove(_clients.FirstOrDefault(x => x.Value == e.Connection.PipeName).Key, out _);
        Debug.WriteLine($"Dalamud client Id {e.Connection.PipeName} disconnected");
    }

    /// <summary>
    ///     If connected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnConnected(object? sender, ConnectionEventArgs<PayloadMessage> e)
    {
        Debug.WriteLine($"Dalamud client Id {e.Connection.PipeName} connected");
    }
}