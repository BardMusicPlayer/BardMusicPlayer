/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Threading.Tasks;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Ui.Utilities;
using BardMusicPlayer.Ui.Views;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class MainViewModel : Screen
    {
        private bool _started;

        protected override void OnViewLoaded()
        {
            var view = (MainView) View;
            BmpPigeonhole.Initialize(Globals.DataPath + @"\Configuration.json");

            LogManager.Initialize(new(view.Log));

            BmpCoffer.Initialize(Globals.DataPath + @"\MusicCatalog.db");

            BmpSeer.Instance.SetupFirewall("BardMusicPlayer");

            BmpSeer.Instance.Start();

            BmpGrunt.Instance.Start();
        }

        protected override void OnClose()
        {
            LogManager.Shutdown();

            // BmpMaestro.Instance.Stop();

            BmpGrunt.Instance.Stop();

            BmpSeer.Instance.Stop();

            BmpSeer.Instance.DestroyFirewall("BardMusicPlayer");

            BmpCoffer.Instance.Dispose();

            BmpPigeonhole.Instance.Dispose();
        }

        public Task<bool> StartUp(bool beta, int build, string commit, string exePath, string resourcePath,
            string dataPath, string[] args)
        {
            if (_started) throw new("Cannot start up twice.");

            Globals.IsBeta       = beta;
            Globals.Build        = build;
            Globals.Commit       = commit;
            Globals.ExePath      = exePath;
            Globals.ResourcePath = resourcePath;
            Globals.DataPath     = dataPath;

            var bootstrapper = new Bootstrapper();
            bootstrapper.Start(args);

            //var mainView = new MainView();
            //mainView.ShowDialog();
            _started = true;
            return Task.FromResult(_started);
        }
    }
}