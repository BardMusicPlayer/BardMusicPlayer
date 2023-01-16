#region

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.DalamudBridge.Helper.Dalamud;

#endregion


namespace BardMusicPlayer.DalamudBridge;

public sealed partial class DalamudBridge
{
    private ConcurrentQueue<DalamudBridgeCommandStruct> _eventQueue;
    private bool _eventQueueOpen;

    private CancellationTokenSource _eventsTokenSource;

    private async Task RunEventsHandler(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            while (_eventQueue.TryDequeue(out var d_event))
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
                    }
                }
                catch
                {
                    // ignored
                }
            }

            await Task.Delay(25, token).ContinueWith(static tsk => { }, token);
        }
    }

    private void StartEventsHandler()
    {
        _eventQueue = new ConcurrentQueue<DalamudBridgeCommandStruct>();
        _eventsTokenSource = new CancellationTokenSource();
        Task.Factory.StartNew(() => RunEventsHandler(_eventsTokenSource.Token), TaskCreationOptions.LongRunning);
        _eventQueueOpen = true;
    }

    private void StopEventsHandler()
    {
        _eventQueueOpen = false;
        _eventsTokenSource.Cancel();
        while (_eventQueue.TryDequeue(out _))
        {
        }
    }

    internal void PublishEvent(DalamudBridgeCommandStruct meastroEvent)
    {
        if (!_eventQueueOpen)
            return;

        _eventQueue.Enqueue(meastroEvent);
    }
}