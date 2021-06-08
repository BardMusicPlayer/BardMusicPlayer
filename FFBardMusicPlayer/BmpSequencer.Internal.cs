using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sanford.Multimedia.Midi;

namespace FFBardMusicPlayer
{
    public class BmpCustomSequencer : IComponent
    {
        private Sequence sequence;
        private readonly List<IEnumerator<int>> enumerators = new List<IEnumerator<int>>();
        private readonly MessageDispatcher dispatcher = new MessageDispatcher();
        private readonly ChannelChaser chaser = new ChannelChaser();
        private readonly ChannelStopper stopper = new ChannelStopper();
        private int tracksPlayingCount;
        private readonly object lockObject = new object();

        public bool IsPlaying { get; private set; }

        public MidiInternalClock InternalClock { get; } = new MidiInternalClock();

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

        public BmpCustomSequencer()
        {
            dispatcher.MetaMessageDispatched += delegate(object sender, MetaMessageEventArgs e)
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

            dispatcher.ChannelMessageDispatched += delegate(object sender, ChannelMessageEventArgs e)
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

        ~BmpCustomSequencer() { Dispose(false); }

        protected virtual void Dispose(bool disposing)
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

                IsPlaying    = true;
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

        protected virtual void OnPlayStatusChange(EventArgs e)
        {
            var handler = PlayStatusChange;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnDisposed(EventArgs e)
        {
            var handler = Disposed;

            if (handler != null)
            {
                handler(this, e);
            }
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
                else if (value < 0)
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
            get { return sequence; }
            set
            {
                #region Require

                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                else if (value.SequenceType == SequenceType.Smpte)
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
}