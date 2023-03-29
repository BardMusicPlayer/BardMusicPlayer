/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Diagnostics;

namespace BardMusicPlayer.Seer.Utilities
{
    public sealed class DalamudManager 
    {
        private static readonly Lazy<DalamudManager> LazyInstance = new(static () => new DalamudManager());

        private readonly object _lock;

        private DalamudManager()
        {
            _lock = new object();

            Trace.UseGlobalLock = false;
            Trace.Listeners.Add(new MachinaLogger());

        }

        public static DalamudManager Instance => LazyInstance.Value;

        public void Dispose()
        {
            lock (_lock)
            {

            }
        }

        #region Accessors

        internal event NameAndHomeworldModeHandler NameAndHomeworld;
        internal delegate void NameAndHomeworldModeHandler(int processId, string Name, int WorldId);
        public void NameAndHomeworldModeEventHandler(int processId, string Name, int WorldId)
        {
            NameAndHomeworld?.Invoke(processId, Name, WorldId);
        }

        internal event EnsembleStartHandler EnsembleStart;
        internal delegate void EnsembleStartHandler(int processId, int code);
        public void EnsembleStartEventHandler(int processId, int code)
        {
            EnsembleStart?.Invoke(processId, code);
        }

        internal event PerformanceModeHandler PerformanceMode;
        internal delegate void PerformanceModeHandler(int processId, int CurrentGroupTone);
        public void PerformanceModeEventHandler(int processId, int CurrentGroupTone)
        {
            PerformanceMode?.Invoke(processId, CurrentGroupTone);
        }
        #endregion

        ~DalamudManager()
        {
            Dispose();
        }
    }
}