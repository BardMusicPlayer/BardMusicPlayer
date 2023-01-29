/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro;

public partial class BmpMaestro
{
    public EventHandler<CurrentPlayPositionEvent> OnPlaybackTimeChanged;
    public EventHandler<MaxPlayTimeEvent> OnSongMaxTime;
    public EventHandler<SongLoadedEvent> OnSongLoaded;
    public EventHandler<bool> OnPlaybackStarted;
    public EventHandler<bool> OnPlaybackStopped;
    public EventHandler<bool> OnPerformerChanged;
    public EventHandler<TrackNumberChangedEvent> OnTrackNumberChanged;
    public EventHandler<OctaveShiftChangedEvent> OnOctaveShiftChanged;
    public EventHandler<PerformerUpdate> OnPerformerUpdate;
    private ConcurrentQueue<MaestroEvent> _eventQueue;
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
                        case CurrentPlayPositionEvent currentPlayPosition:
                            OnPlaybackTimeChanged(this, currentPlayPosition);
                            break;
                        case MaxPlayTimeEvent maxPlayTime:
                            OnSongMaxTime(this, maxPlayTime);
                            break;
                        case SongLoadedEvent songloaded:
                            OnSongLoaded?.Invoke(this, songloaded);
                            break;
                        case PlaybackStartedEvent playbackStarted:
                            OnPlaybackStarted?.Invoke(this, playbackStarted.Started);
                            break;
                        case PlaybackStoppedEvent playbackStopped:
                            OnPlaybackStopped?.Invoke(this, playbackStopped.Stopped);
                            break;
                        case PerformersChangedEvent performerChanged:
                            OnPerformerChanged?.Invoke(this, performerChanged.Changed);
                            break;
                        case TrackNumberChangedEvent trackNumberChanged:
                            OnTrackNumberChanged?.Invoke(this, trackNumberChanged);
                            break;
                        case OctaveShiftChangedEvent octaveShiftChanged:
                            OnOctaveShiftChanged?.Invoke(this, octaveShiftChanged);
                            break;
                        case PerformerUpdate performerUpdate:
                            OnPerformerUpdate?.Invoke(this, performerUpdate);
                            break;

                    }
                }
                catch
                {
                    // ignored
                }
            }
            await Task.Delay(25, token).ContinueWith(tsk=> { }, token);
        }
    }

    private CancellationTokenSource _eventsTokenSource;

    private void StartEventsHandler()
    {
        _eventQueue        = new ConcurrentQueue<MaestroEvent>();
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

    internal void PublishEvent(MaestroEvent maestroEvent)
    {
        if (!_eventQueueOpen)
            return;

        _eventQueue.Enqueue(maestroEvent);
    }
}