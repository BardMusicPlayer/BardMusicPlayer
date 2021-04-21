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
