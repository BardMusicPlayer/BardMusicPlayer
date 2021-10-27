using BardMusicPlayer.Transmogrify.Song;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro
{
    public partial class BmpMaestro
    {
        private async Task RunEventsHandler(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1, token);
            }
        }

        private CancellationTokenSource _eventsTokenSource;

        private void StartEventsHandler()
        {
            _eventsTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => RunEventsHandler(_eventsTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        private void StopEventsHandler()
        {
            _eventsTokenSource.Cancel();
        }

        public void PlayWithLocalPerformer(Task<BmpSong> bmpSong, int v)
        {
            throw new NotImplementedException();
        }
    }
}