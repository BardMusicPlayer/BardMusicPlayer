using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sanford.Multimedia.Midi;

namespace FFBardMusicPlayer {
	public class BmpCustomSequencer : IComponent {
		private Sequence sequence = null;

		private List<IEnumerator<int>> enumerators = new List<IEnumerator<int>>();

		private MessageDispatcher dispatcher = new MessageDispatcher();

		private ChannelChaser chaser = new ChannelChaser();

		private ChannelStopper stopper = new ChannelStopper();

		private MidiInternalClock clock = new MidiInternalClock();

		private int tracksPlayingCount;

		private readonly object lockObject = new object();

		private bool playing = false;
		public bool IsPlaying {
			get {
				return playing;
			}
		}

		public MidiInternalClock InternalClock {
			get {
				return clock;
			}
		}

		private bool disposed = false;

		private ISite site = null;

		#region Events

		public event EventHandler PlayStatusChange;
		public event EventHandler PlayEnded;

		public event EventHandler<ChannelMessageEventArgs> ChannelMessagePlayed {
			add {
				dispatcher.ChannelMessageDispatched += value;
			}
			remove {
				dispatcher.ChannelMessageDispatched -= value;
			}
		}

		public event EventHandler<SysExMessageEventArgs> SysExMessagePlayed {
			add {
				dispatcher.SysExMessageDispatched += value;
			}
			remove {
				dispatcher.SysExMessageDispatched -= value;
			}
		}

		public event EventHandler<MetaMessageEventArgs> MetaMessagePlayed {
			add {
				dispatcher.MetaMessageDispatched += value;
			}
			remove {
				dispatcher.MetaMessageDispatched -= value;
			}
		}

		public event EventHandler<ChasedEventArgs> Chased {
			add {
				chaser.Chased += value;
			}
			remove {
				chaser.Chased -= value;
			}
		}

		public event EventHandler<StoppedEventArgs> Stopped {
			add {
				stopper.Stopped += value;
			}
			remove {
				stopper.Stopped -= value;
			}
		}

		#endregion

		public BmpCustomSequencer() {
			dispatcher.MetaMessageDispatched += delegate (object sender, MetaMessageEventArgs e) {
				if(e.Message.MetaType == MetaType.EndOfTrack) {
					tracksPlayingCount--;

					if(tracksPlayingCount == 0) {
						Stop();
					}
				} else {
					clock.Process(e.Message);
				}
			};

			dispatcher.ChannelMessageDispatched += delegate (object sender, ChannelMessageEventArgs e) {
				stopper.Process(e.Message);
			};

			clock.Tick += delegate (object sender, EventArgs e) {
				lock(lockObject) {
					if(!playing) {
						return;
					}

					foreach(IEnumerator<int> enumerator in enumerators) {
						enumerator.MoveNext();
					}
				}
				if(tracksPlayingCount == 0) {
					PlayEnded?.Invoke(this, EventArgs.Empty);
				}
			};
		}

		~BmpCustomSequencer() {
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				lock(lockObject) {
					Stop();

					clock.Dispose();

					disposed = true;

					GC.SuppressFinalize(this);
				}
			}
		}

		public void Stop() {
			#region Require

			if(disposed) {
				throw new ObjectDisposedException(this.GetType().Name);
			}

			#endregion

			lock(lockObject) {
				Pause();
				Position = 0;

				OnPlayStatusChange(EventArgs.Empty);
			}
		}

		public void Play() {
			#region Require

			if(disposed) {
				throw new ObjectDisposedException(this.GetType().Name);
			}

			#endregion

			#region Guard

			if(Sequence == null) {
				return;
			}

			#endregion

			lock(lockObject) {
				Pause();

				enumerators.Clear();

				foreach(Track t in Sequence) {
					enumerators.Add(t.TickIterator(Position, chaser, dispatcher).GetEnumerator());
				}

				tracksPlayingCount = Sequence.Count;

				playing = true;
				clock.Ppqn = sequence.Division;
				clock.Continue();

				OnPlayStatusChange(EventArgs.Empty);
			}
		}

		public void Pause() {
			#region Require

			if(disposed) {
				throw new ObjectDisposedException(this.GetType().Name);
			}

			#endregion

			lock(lockObject) {
				#region Guard

				if(!playing) {
					return;
				}

				#endregion

				playing = false;

				clock.Stop();
				stopper.AllSoundOff();

				OnPlayStatusChange(EventArgs.Empty);
			}
		}

		protected virtual void OnPlayStatusChange(EventArgs e) {
			EventHandler handler = PlayStatusChange;

			if(handler != null) {
				handler(this, e);
			}
		}

		protected virtual void OnDisposed(EventArgs e) {
			EventHandler handler = Disposed;

			if(handler != null) {
				handler(this, e);
			}
		}

		public float Speed {
			get {
				return clock.TempoSpeed;
			}
			set {
				clock.TempoSpeed = value;
			}
		}

		public int Length {
			get {
				#region Require

				if(disposed) {
					throw new ObjectDisposedException(this.GetType().Name);
				}

				#endregion

				return sequence.GetLength();
			}
		}

		public int Position {
			get {
				#region Require

				if(disposed) {
					throw new ObjectDisposedException(this.GetType().Name);
				}

				#endregion

				return clock.Ticks;
			}
			set {
				#region Require

				if(disposed) {
					throw new ObjectDisposedException(this.GetType().Name);
				} else if(value < 0) {
					throw new ArgumentOutOfRangeException();
				}

				#endregion

				bool wasPlaying;

				lock(lockObject) {
					wasPlaying = playing;

					Pause();

					clock.SetTicks(value);
				}

				lock(lockObject) {
					if(wasPlaying) {
						Play();
					}
				}
			}
		}

		public Sequence Sequence {
			get {
				return sequence;
			}
			set {
				#region Require

				if(value == null) {
					throw new ArgumentNullException();
				} else if(value.SequenceType == SequenceType.Smpte) {
					throw new NotSupportedException();
				}

				#endregion

				lock(lockObject) {
					Stop();
					sequence = value;
				}
			}
		}

		#region IComponent Members

		public event EventHandler Disposed;

		public ISite Site {
			get {
				return site;
			}
			set {
				site = value;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			#region Guard

			if(disposed) {
				return;
			}

			#endregion

			Dispose(true);
		}

		#endregion
	}
}