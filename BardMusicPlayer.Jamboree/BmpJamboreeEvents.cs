using BardMusicPlayer.Jamboree.Events;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Jamboree
{
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
                                if (OnPartyCreated == null)
                                    break;
                                OnPartyCreated(this, partyCreated);
                                break;
                            case PartyLogEvent partyLog:
                                if (OnPartyLog == null)
                                    break;
                                OnPartyLog(this, partyLog);
                                break;
                            case PartyDebugLogEvent partyDebugLog:
                                if (OnPartyDebugLog == null)
                                    break;
                                OnPartyDebugLog(this, partyDebugLog);
                                break;
                            case PartyConnectionChangedEvent connectionChanged:
                                if (OnPartyConnectionChanged == null)
                                    break;
                                OnPartyConnectionChanged(this, connectionChanged);
                                break;
                            case PartyChangedEvent partyChanged:
                                if (OnPartyChanged == null)
                                    break;
                                OnPartyChanged(this, partyChanged);
                                break;
                            case PerformanceStartEvent performanceStart:
                                if (OnPerformanceStart == null)
                                    break;
                                OnPerformanceStart(this, performanceStart);
                                break;
                        };
                    }
                    catch
                    { }
                }
                await Task.Delay(25, token).ContinueWith(tsk => { });
            }
        }

        private CancellationTokenSource _eventsTokenSource;

        private void StartEventsHandler()
        {
            _eventQueue = new ConcurrentQueue<JamboreeEvent>();
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

        internal void PublishEvent(JamboreeEvent meastroEvent)
        {
            if (!_eventQueueOpen)
                return;

            _eventQueue.Enqueue(meastroEvent);
        }
    }
}
