/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Threading.Tasks;
using BardMusicPlayer.Ui.Utilities;

namespace BardMusicPlayer.Ui
{
    public class Main
    {
        private bool _started;
        public Task<bool> StartUp(bool beta, int build, string commit, string exePath, string resourcePath, string dataPath, string[] args)
        {
            if (_started) throw new Exception("Cannot start up twice.");

            Globals.IsBeta = beta;
            Globals.Build = build;
            Globals.Commit = commit;
            Globals.ExePath = exePath;
            Globals.ResourcePath = resourcePath;
            Globals.DataPath = dataPath;

            var mainView = new MainView();
            mainView.ShowDialog();
            _started = true;
            return Task.FromResult(_started);
        }
    }
}
