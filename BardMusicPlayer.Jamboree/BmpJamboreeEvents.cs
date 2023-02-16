using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Jamboree.Events;

namespace BardMusicPlayer.Jamboree;

public partial class BmpJamboree
{
    public EventHandler<PartyCreatedEvent> OnPartyCreated;
    public EventHandler<PartyLogEvent> OnPartyLog;
    public EventHandler<PartyDebugLogEvent> OnPartyDebugLog;
    public EventHandler<PartyConnectionChangedEvent> OnPartyConnectionChanged;
    public EventHandler<PartyChangedEvent> OnPartyChanged;
    public EventHandler<PerformanceStartEvent> OnPerformanceStart;

    private ConcurrentQueue<JamboreeEvent> _eventQueue;
    private bool _eventQueueOpen;

    private async Task RunEventsHandler(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            while (_eventQueue.TryDequeue(out var meastroEvent))
            {
                if (token.IsCancellationRequested)
                    break;

                try
                {
                    switch (meastroEvent)
                    {
                        case PartyCreatedEvent partyCreated:
                            OnPartyCreated?.Invoke(this, partyCreated);
                            break;
                        case PartyLogEvent partyLog:
                            OnPartyLog?.Invoke(this, partyLog);
                            break;
                        case PartyDebugLogEvent partyDebugLog:
                            OnPartyDebugLog?.Invoke(this, partyDebugLog);
                            break;
                        case PartyConnectionChangedEvent connectionChanged:
                            OnPartyConnectionChanged?.Invoke(this, connectionChanged);
                            break;
                        case PartyChangedEvent partyChanged:
                            OnPartyChanged?.Invoke(this, partyChanged);
                            break;
                        case PerformanceStartEvent performanceStart:
                            OnPerformanceStart?.Invoke(this, performanceStart);
                            break;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            await Task.Delay(25, token).ContinueWith(tsk => { }, token);
        }
    }

    private CancellationTokenSource _eventsTokenSource;

    private void StartEventsHandler()
    {
        _eventQueue        = new ConcurrentQueue<JamboreeEvent>();
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

    internal void PublishEvent(JamboreeEvent maestroEvent)
    {
        if (!_eventQueueOpen)
            return;

        _eventQueue.Enqueue(maestroEvent);
    }
}