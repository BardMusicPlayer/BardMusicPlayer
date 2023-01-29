/*
 * Copyright(c) 2022 Parulina, trotlinebeercan, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sanford.Multimedia.Midi;

namespace BardMusicPlayer.Maestro.Sequencing;

public class Sequencer_Internal : IComponent
{
    private Sequence sequence;

    private List<IEnumerator<int>> enumerators = new();

    private MessageDispatcher dispatcher = new();

    private ChannelChaser chaser = new();

    private ChannelStopper stopper = new();
    private int tracksPlayingCount;

    private readonly object lockObject = new();

    public bool IsPlaying { get; private set; }

    public MidiInternalClock InternalClock { get; } = new();

    private bool disposed;

    #region Events

    public event EventHandler PlayStatusChange;
    public event EventHandler PlayEnded;

    public event EventHandler<ChannelMessageEventArgs> ChannelMessagePlayed
    {
        add => dispatcher.ChannelMessageDispatched += value;
        remove => dispatcher.ChannelMessageDispatched -= value;
    }

    public event EventHandler<SysExMessageEventArgs> SysExMessagePlayed
    {
        add => dispatcher.SysExMessageDispatched += value;
        remove => dispatcher.SysExMessageDispatched -= value;
    }

    public event EventHandler<MetaMessageEventArgs> MetaMessagePlayed
    {
        add => dispatcher.MetaMessageDispatched += value;
        remove => dispatcher.MetaMessageDispatched -= value;
    }

    public event EventHandler<ChasedEventArgs> Chased
    {
        add => chaser.Chased += value;
        remove => chaser.Chased -= value;
    }

    public event EventHandler<StoppedEventArgs> Stopped
    {
        add => stopper.Stopped += value;
        remove => stopper.Stopped -= value;
    }

    #endregion

    public Sequencer_Internal()
    {
        dispatcher.MetaMessageDispatched += delegate (object sender, MetaMessageEventArgs e)
        {
            if (e.Message.MetaType == MetaType.EndOfTrack)
            {
                tracksPlayingCount--;

                if (tracksPlayingCount == 0)
                {
                    Stop();
                }
            }
            else
            {
                InternalClock.Process(e.Message);
            }
        };

        dispatcher.ChannelMessageDispatched += delegate (object sender, ChannelMessageEventArgs e)
        {
            stopper.Process(e.Message);
        };

        InternalClock.Tick += delegate
        {
            lock (lockObject)
            {
                if (!IsPlaying)
                {
                    return;
                }

                foreach (var enumerator in enumerators)
                {
                    enumerator.MoveNext();
                }
            }
            if (tracksPlayingCount == 0)
            {
                PlayEnded?.Invoke(this, EventArgs.Empty);
            }
        };
    }

    ~Sequencer_Internal()
    {
        Dispose(false);
    }

    protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (lockObject)
            {
                Stop();

                InternalClock.Dispose();
                disposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }

    public void Stop()
    {
        #region Require

        if (disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        lock (lockObject)
        {
            Pause();
            Position = 0;

            OnPlayStatusChange(EventArgs.Empty);
        }
    }

    public void Play()
    {
        #region Require

        if (disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region Guard

        if (Sequence == null)
        {
            return;
        }

        #endregion

        lock (lockObject)
        {
            Pause();

            enumerators.Clear();

            foreach (var t in Sequence)
            {
                enumerators.Add(t.TickIterator(Position, chaser, dispatcher).GetEnumerator());
            }

            tracksPlayingCount = Sequence.Count;

            IsPlaying          = true;
            InternalClock.Ppqn = sequence.Division;
            InternalClock.Continue();

            OnPlayStatusChange(EventArgs.Empty);
        }
    }

    public void Pause()
    {
        #region Require

        if (disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        lock (lockObject)
        {
            #region Guard

            if (!IsPlaying)
            {
                return;
            }

            #endregion

            IsPlaying = false;

            InternalClock.Stop();
            stopper.AllSoundOff();

            OnPlayStatusChange(EventArgs.Empty);
        }
    }

    protected void OnPlayStatusChange(EventArgs e)
    {
        var handler = PlayStatusChange;

        handler?.Invoke(this, e);
    }

    protected void OnDisposed(EventArgs e)
    {
        var handler = Disposed;

        handler?.Invoke(this, e);
    }

    public float Speed
    {
        get => InternalClock.TempoSpeed;
        set => InternalClock.TempoSpeed = value;
    }

    public int Length
    {
        get
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            #endregion

            return sequence.GetLength();
        }
    }

    public int Position
    {
        get
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            #endregion

            return InternalClock.Ticks;
        }
        set
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            #endregion

            bool wasPlaying;

            lock (lockObject)
            {
                wasPlaying = IsPlaying;

                Pause();

                InternalClock.SetTicks(value);
            }

            lock (lockObject)
            {
                if (wasPlaying)
                {
                    Play();
                }
            }
        }
    }

    public Sequence Sequence
    {
        get
        {
            return sequence;
        }
        set
        {
            #region Require

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (value.SequenceType == SequenceType.Smpte)
            {
                throw new NotSupportedException();
            }

            #endregion

            lock (lockObject)
            {
                Stop();
                sequence = value;
            }
        }
    }

    #region IComponent Members

    public event EventHandler Disposed;

    public ISite Site { get; set; }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        #region Guard

        if (disposed)
        {
            return;
        }

        #endregion

        Dispose(true);
    }

    #endregion
}