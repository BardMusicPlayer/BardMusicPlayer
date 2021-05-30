/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Seer.Reader.Backend;

namespace BardMusicPlayer.Seer.Reader
{
    internal class ReaderHandler : IDisposable
    {
        private readonly IReaderBackend _readerBackend;
        internal readonly Game Game;
        private CancellationTokenSource _cts;
        private Task _task;

        internal ReaderHandler(Game game, IReaderBackend readerBackend)
        {
            Game                         = game;
            _readerBackend               = readerBackend;
            _readerBackend.ReaderHandler = this;
            StartBackend();
        }

        ~ReaderHandler() { Dispose(); }

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
            if (_task != null)
                throw new BmpSeerBackendAlreadyRunningException(Game.Process.Id, _readerBackend.ReaderBackendType);

            _cts  = new CancellationTokenSource();
            _task = Task.Factory.StartNew(() => _readerBackend.Loop(_cts.Token), TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops the internal IBackend thread.
        /// </summary>
        internal void StopBackend()
        {
            if (_task == null) return;

            _cts.Cancel();
            _task = null;
        }
    }
}