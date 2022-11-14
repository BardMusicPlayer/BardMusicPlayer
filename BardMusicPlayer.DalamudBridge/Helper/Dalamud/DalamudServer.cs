/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

#nullable enable
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BardMusicPlayer.Quotidian.Structs;
using H.Formatters;
using H.Pipes;
using H.Pipes.AccessControl;
using H.Pipes.Args;
using Newtonsoft.Json;

namespace BardMusicPlayer.DalamudBridge.Helper.Dalamud
{
    public class Message
    {
        public MessageType msgType { get; set; } = MessageType.None;
        public int  msgChannel { get; set; } = 0;
        public string message { get; set; } = "";
    }

    internal class DalamudServer : IDisposable
    {
        private readonly PipeServer<Message> _pipe;
        private readonly ConcurrentDictionary<int, string> _clients;

        /// <summary>
        /// 
        /// </summary>
        internal DalamudServer()
        {
            _clients = new ConcurrentDictionary<int, string>();
            _pipe = new PipeServer<Message>("BMP-DalamudBridge", formatter: new NewtonsoftJsonFormatter());
            _pipe.ClientConnected += OnConnected;
            _pipe.ClientDisconnected += OnDisconnected;
            _pipe.MessageReceived += OnMessage;
            _pipe.AllowUsersReadWrite();
            Start();
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
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        internal bool IsConnected(int pid) => _clients.ContainsKey(pid) && _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected) is not null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal bool SendChat(int pid, ChatMessageChannelType chanType, string text)
        {
            if (!IsConnected(pid))
                return false;

            _pipe.ConnectedClients.FirstOrDefault(x => x.PipeName == _clients[pid] && x.IsConnected)?.WriteAsync(new Message
            {
                msgType = MessageType.Chat,
                msgChannel = chanType.ChannelCode,
                message = text
            });
            return true;
        }

        /// <summary>
        /// If message from client rec
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessage(object sender, ConnectionMessageEventArgs<Message?> e)
        {
            Message? inMsg = e.Message as Message;
            if (inMsg == null)
                return;

            if (inMsg.msgType == MessageType.Handshake)
            {
                int t = Convert.ToInt32(inMsg.message);
                _clients.TryAdd(t, e.Connection.PipeName);
                Debug.WriteLine($"Dalamud client Id {e.Connection.PipeName} {t} connected");
            }
        }

        /// <summary>
        /// If client disconnected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisconnected(object sender, ConnectionEventArgs<Message> e)
        {
            if (_clients.Values.Contains(e.Connection.PipeName)) 
                _clients.TryRemove(_clients.FirstOrDefault(x => x.Value == e.Connection.PipeName).Key, out _);
            Debug.WriteLine($"Dalamud client Id {e.Connection.PipeName} disconnected");
        }

        /// <summary>
        /// If connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnected(object sender, ConnectionEventArgs<Message> e)
        {
            Debug.WriteLine($"Dalamud client Id {e.Connection.PipeName} connected");
        }

        /// <summary>
        /// Disposing
        /// </summary>
        public void Dispose()
        {
            try
            {
                Stop();
                _pipe.MessageReceived -= OnMessage;
                _pipe.ClientConnected -= OnDisconnected;
                _pipe.ClientDisconnected -= OnConnected;
                _pipe.DisposeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dalamud error: {ex.Message}");
            }
            _clients.Clear();
        }
    }
}
