using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FFBardMusicPlayer {

	public class NoteProcessingBase<T> {
		public EventHandler<T> NoteEvent;
		protected BmpNoteTimers<T> noteTimers = new BmpNoteTimers<T>();
		public long Tick {
			get {
				return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			}
		}

		public long MaxTick {
			get {
				if(noteTimers.timestamps.IsEmpty) {
					return 0;
				}
				return noteTimers.timestamps.Values.Max();
			}
		}

		public bool HasTimer(T t) {
			return noteTimers.TryGetTimer(t, out Timer timer);
		}
	}


	public class NoteDoubleDetection<T> : NoteProcessingBase<T> {

		public void Clear() {
			noteTimers.Clear();
		}

		public void OffKey(T key) {

			noteTimers.SetTimestamp(key, this.Tick);
		}

		public bool OnKey(T key) {

			long timestamp = noteTimers.TakeAndUpdateTimestamp(key, this.Tick);

			// BUGFIX Compare against all notes
			timestamp = this.MaxTick;

			if(timestamp > 0) {

				int tooFastDelay = 25;
				bool tooFastSuccession = (Math.Abs(this.Tick - timestamp) < tooFastDelay);
				if(tooFastSuccession) {

					if(noteTimers.TryRemoveTimer(key, out Timer newKeyTimer)) {
						newKeyTimer.Dispose();
					}

					newKeyTimer = new Timer {
						Interval = tooFastDelay,
						Enabled = true,
					};
					newKeyTimer.Elapsed += delegate (object o, System.Timers.ElapsedEventArgs e) {
						if(noteTimers.TryRemoveTimer(key, out Timer timer)) {
							timer.Dispose();
							NoteEvent?.Invoke(this, key);
						}
					};

					noteTimers.SetTimer(key, newKeyTimer);
					return true;
				}
			}
			return false;
		}
	}


	public class BmpNoteTimers<T> {

		public class Timers : ConcurrentDictionary<T, Timer> { }
		public class Timestamps : ConcurrentDictionary<T, long> { }

		public Timers timers = new Timers();
		public Timestamps timestamps = new Timestamps();

		public bool TryGetTimer(T t, out Timer timer) {
			return timers.TryGetValue(t, out timer);
		}
		public bool TryGetTimestamp(T t, out long timestamp) {
			return timestamps.TryGetValue(t, out timestamp);
		}
		public bool TryRemoveTimer(T t, out Timer timer) {
			return timers.TryRemove(t, out timer);
		}
		public bool TryRemoveTimestamp(T t, out long timestamp) {
			return timestamps.TryRemove(t, out timestamp);
		}

		public void Clear() {
			while(!timers.IsEmpty) {
				if(timers.TryRemove(timers.First().Key, out Timer tmr)) {
					tmr.Stop();
				}
			}
		}

		public void SetTimer(T t, Timer timer) {
			if(TryGetTimer(t, out Timer timer2)) {
				timer2.Dispose();
			}
			timers[t] = timer;
		}

		public void SetTimestamp(T t, long timestamp) {
			timestamps[t] = timestamp;
		}

		public long TakeAndUpdateTimestamp(T t, long timestamp) {
			long ts = 0;
			if(this.TryGetTimestamp(t, out ts)) {
				timestamps[t] = timestamp;
			}
			return ts;
		}

	}
}
