using System;
using BardMusicPlayer.Seer.Reader;
namespace BardMusicPlayer.Seer
{
    public class Game : IDisposable
    {
        public readonly Storage Storage;
        internal readonly MemoryReader MemoryReader;
        internal readonly NetworkReader NetworkReader;
        internal readonly DatReader DatReader;

        internal Game(int pid)
        {
            Storage = new Storage(pid);
            MemoryReader = new MemoryReader(this);
            NetworkReader = new NetworkReader(this);
            DatReader = new DatReader(this);
        }

        /// <summary>
        /// Called when a Reader pushes a new, processed Event.
        /// </summary>
        /// <param name="newEvent">The Event that was pushed.</param>
        internal void OnEventReceived(Event.Event newEvent)
        {
            // make sure it's valid to begin with
            if (!newEvent.IsValid())
                return;

            // based on event type, we'll need to determine what we'll take
            // from it. if anything at all
            if (newEvent.EventType == typeof(Event.NetworkEvent))
            {
                // network has highest priority
            }
            else if (newEvent.EventType == typeof(Event.MemoryEvent))
            {
                // memory has second highest priority
            }
            else if (newEvent.EventType == typeof(Event.DatEvent))
            {
                // dat has third highest priority
            }
            else
            {
                throw new Exception("Unknown event received in Game.OnEventReceived");
            }

            // TODO: determine if we want to pass the casted events to Storage
            //       or if we want to modify the fields of Storage here
            // this.Storage.UpdateData(...); ????

            Seer.Instance.OnGameUpdated(this);
        }

        ~Game() => Dispose();
        public void Dispose()
        {
            try
            {
                DatReader.Dispose();
            }
            catch (Exception exception)
            {

            }
            try
            {
                NetworkReader.Dispose();
            }
            catch (Exception exception)
            {

            }
            try
            {
                MemoryReader.Dispose();
            }
            catch (Exception exception)
            {

            }
            try
            {
                Storage.Dispose();
            }
            catch (Exception exception)
            {

            }
        }
    }
}
