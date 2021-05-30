/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Seer.Reader;
using BardMusicPlayer.Seer.Reader.Backend.DatFile;
using BardMusicPlayer.Seer.Reader.Backend.Machina;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan;

namespace BardMusicPlayer.Seer
{
    public partial class Game : IDisposable, IEquatable<Game>
    {
        private readonly string _uuid;

        // readers
        internal ReaderHandler DatReader;
        internal ReaderHandler MemoryReader;
        internal ReaderHandler NetworkReader;

        // reader events
        private Dictionary<Type, long> _eventDedupeHistory;
        private ConcurrentQueue<SeerEvent> _eventQueueHighPriority;
        private ConcurrentQueue<SeerEvent> _eventQueueLowPriority;
        private bool _eventQueueOpen;

        // reader events processor
        private CancellationTokenSource _eventTokenSource;

        internal Game(Process process)
        {
            _uuid   = Guid.NewGuid().ToString();
            Process = process;
        }

        internal bool Initialize()
        {
            try
            {
                if (Process is null || Process.Id < 1 || Pid != 0)
                {
                    BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid,
                        new BmpSeerException("Game process is null or already initialized.")));
                    return false;
                }

                Pid = Process.Id;
                InitInformation();

                _eventDedupeHistory     = new Dictionary<Type, long>();
                _eventQueueHighPriority = new ConcurrentQueue<SeerEvent>();
                _eventQueueLowPriority  = new ConcurrentQueue<SeerEvent>();
                _eventQueueOpen         = true;

                DatReader     = new ReaderHandler(this, new DatFileReaderBackend(1));
                MemoryReader  = new ReaderHandler(this, new SharlayanReaderBackend(1));
                NetworkReader = new ReaderHandler(this, new MachinaReaderBackend(1));

                _eventTokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(() => RunEventQueue(_eventTokenSource.Token), TaskCreationOptions.LongRunning);

                BmpSeer.Instance.PublishEvent(new GameStarted(this, Pid));
            }
            catch (Exception ex)
            {
                BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
                return false;
            }

            return true;
        }

        internal void PublishEvent(SeerEvent seerEvent)
        {
            if (!_eventQueueOpen) return;
            if (seerEvent.HighPriority) _eventQueueHighPriority.Enqueue(seerEvent);
            else _eventQueueLowPriority.Enqueue(seerEvent);
        }

        private async Task RunEventQueue(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                while (_eventQueueHighPriority.TryDequeue(out var high))
                {
                    try
                    {
                        OnEventReceived(high);
                    }
                    catch (Exception ex)
                    {
                        BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
                    }
                }

                if (_eventQueueLowPriority.TryDequeue(out var low))
                {
                    try
                    {
                        OnEventReceived(low);
                    }
                    catch (Exception ex)
                    {
                        BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
                    }
                }

                await Task.Delay(1, token);
            }
        }

        ~Game() { Dispose(); }

        public void Dispose()
        {
            BmpSeer.Instance.PublishEvent(new GameStopped(Pid));

            _eventQueueOpen = false;
            _eventTokenSource.Cancel();

            try
            {
                DatReader?.Dispose();
            }
            catch (Exception ex)
            {
                BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
            }
            try
            {
                MemoryReader?.Dispose();
            }
            catch (Exception ex)
            {
                BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
            }
            try
            {
                NetworkReader?.Dispose();
            }
            catch (Exception ex)
            {
                BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
            }
            try
            {
                while (_eventQueueHighPriority.TryDequeue(out _)) { }
                while (_eventQueueLowPriority.TryDequeue(out _)) { }
                _eventDedupeHistory.Clear();
            } catch (Exception ex)
            {
                BmpSeer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
            }
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Game) obj);
        }

        public bool Equals(Game other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _uuid == other._uuid;
        }

        public override int GetHashCode() => _uuid != null ? _uuid.GetHashCode() : 0;
        public static bool operator ==(Game game, Game otherGame) => game is not null && game.Equals(otherGame);
        public static bool operator !=(Game game, Game otherGame) => game is not null && !game.Equals(otherGame);
    }
}
