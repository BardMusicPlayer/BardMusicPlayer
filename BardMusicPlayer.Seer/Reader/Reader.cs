using System;
using System.Threading;

namespace BardMusicPlayer.Seer.Reader
{
    internal abstract class Reader : IDisposable
    {
        protected readonly Game Game;
        internal Reader(Game game)
        {
            this.Game = game;

            this.StartReader();
        }
        ~Reader()
        {
            this.StopReader();
            Dispose();
        }
        public abstract void Dispose();

        // threading variables
        private bool ShouldRunReaderThread;
        private Thread ReaderThread;

        /// <summary>
        /// The core function in each external Reader to do what it needs to do.
        /// </summary>
        /// <returns>False, when parent Reader wants to stop the thread.</returns>
        protected abstract bool RunLoop();

        /// <summary>
        /// Used to notify the internal Reader how long to sleep after each loop.
        /// </summary>
        /// /// <returns>False, when parent Reader wants to stop the thread.</returns>
        protected abstract int SleepTimeInMs();

        private void RunReader()
        {
            while (this.ShouldRunReaderThread)
            {
                this.ShouldRunReaderThread = !this.RunLoop();
                Thread.Sleep(this.SleepTimeInMs());
            }

            this.ReaderThread = null;
        }

        /// <summary>
        /// Starts the internal Reader thread.
        /// </summary>
        protected void StartReader()
        {
            if (this.ReaderThread != null)
            {
                // TODO: throw
                return;
            }

            this.ShouldRunReaderThread = true;
            ReaderThread = new Thread(new ThreadStart(RunReader));
            ReaderThread.Start();
        }

        /// <summary>
        /// Stops the internal Reader thread.
        /// </summary>
        protected void StopReader()
        {
            if (this.ReaderThread == null)
            {
                return;
            }

            this.ShouldRunReaderThread = false;
            this.ReaderThread.Join();
            this.ReaderThread = null;
        }

        /// <summary>
        /// Pushes an event from the Reader post RunLoop to the parent Game object.
        /// </summary>
        /// <param name="newEvent">The event to pass to Game.</param>
        protected void PushEvent(Event.Event newEvent)
        {
            this.Game.OnEventReceived(newEvent);
        }
    }
}
