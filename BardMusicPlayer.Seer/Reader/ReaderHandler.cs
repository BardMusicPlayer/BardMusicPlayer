/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Threading;
using BardMusicPlayer.Seer.Reader.Backend;

namespace BardMusicPlayer.Seer.Reader
{
    internal class ReaderHandler : IDisposable
    {
        private readonly IReaderBackend _readerBackend;
        internal readonly Game Game;
        private CancellationTokenSource _cancellationTokenSource;
        private Thread _thread;

        internal ReaderHandler(Game game, IReaderBackend readerBackend)
        {
            Game = game;
            _readerBackend = readerBackend;
            _readerBackend.ReaderHandler = this;
            StartBackend();
        }

        ~ReaderHandler()
        {
            Dispose();
        }

        public void Dispose()
        {
            StopBackend();
            _readerBackend.Dispose();
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Starts the internal IBackend thread.
        /// </summary>
        internal void StartBackend()
        {
            if (_thread != null) throw new BmpSeerBackendAlreadyRunningException(Game.Process.Id, _readerBackend.ReaderBackendType);

            _cancellationTokenSource = new CancellationTokenSource();
            _readerBackend.CancellationToken = _cancellationTokenSource.Token;
            _thread = new Thread(_readerBackend.Loop) { IsBackground = true };
            _thread.Start();
        }

        /// <summary>
        /// Stops the internal IBackend thread.
        /// </summary>
        internal void StopBackend()
        {
            if (_thread == null) return;
            _cancellationTokenSource.Cancel();
            if(!_thread.Join(500)) _thread.Abort();
            _thread = null;
        }
    }
}
