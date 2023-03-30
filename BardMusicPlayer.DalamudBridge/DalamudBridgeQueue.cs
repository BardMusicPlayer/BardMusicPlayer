/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using BardMusicPlayer.DalamudBridge.Helper.Dalamud;

namespace BardMusicPlayer.DalamudBridge;

public sealed partial class DalamudBridge
{
    private ConcurrentQueue<DalamudBridgeCommandStruct>? _eventQueue;
    private bool _eventQueueOpen;

    private CancellationTokenSource? _eventsTokenSource;

    private async Task RunEventsHandler(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            while (_eventQueue != null && _eventQueue.TryDequeue(out var d_event))
            {
                if (token.IsCancellationRequested)
                    break;

                try
                {
                    switch (d_event.messageType)
                    {
                        case MessageType.Chat:
                            await d_event.game.SendText(d_event.chatType, d_event.TextData);
                            break;
                        case MessageType.Instrument:
                            await d_event.game.OpenInstrument(d_event.IntData);
                            break;
                        case MessageType.AcceptReply:
                            await d_event.game.AcceptEnsemble(d_event.BoolData);
                            break;
                        case MessageType.NoteOn:
                        case MessageType.NoteOff:
                            _ = d_event.game.SendNote(d_event.IntData, d_event.BoolData);
                            break;
                        case MessageType.ProgramChange:
                            _ = d_event.game.SendProgChange(d_event.IntData);
                            break;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            await Task.Delay(1, token).ContinueWith(static tsk => { }, token);
        }
    }

    private void StartEventsHandler()
    {
        _eventQueue        = new ConcurrentQueue<DalamudBridgeCommandStruct>();
        _eventsTokenSource = new CancellationTokenSource();
        Task.Factory.StartNew(() => RunEventsHandler(_eventsTokenSource.Token), TaskCreationOptions.LongRunning);
        _eventQueueOpen = true;
    }

    private void StopEventsHandler()
    {
        _eventQueueOpen = false;
        _eventsTokenSource?.Cancel();
        while (_eventQueue != null && _eventQueue.TryDequeue(out _))
        {
        }
    }

    internal void PublishEvent(DalamudBridgeCommandStruct maestroEvent)
    {
        if (!_eventQueueOpen)
            return;

        _eventQueue?.Enqueue(maestroEvent);
    }
}