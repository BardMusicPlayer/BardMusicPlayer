using System;
﻿using System.Threading.Tasks;
using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Ui.Utilities;
using BardMusicPlayer.Ui.ViewModels;
using Stylet;

namespace BardMusicPlayer.Ui
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {
        private bool _started;
        private static readonly Lazy<Bootstrapper> LazyInstance = new(() => new Bootstrapper());
        public static Bootstrapper Instance => LazyInstance.Value;

        protected override void OnStart()
        {
            BmpPigeonhole.Initialize(Globals.DataPath + @"\Configuration.json");

            // var view = (MainView)View;
            // LogManager.Initialize(new(view.Log));

            BmpCoffer.Initialize(Globals.DataPath + @"\MusicCatalog.db");

            BmpSeer.Instance.SetupFirewall("BardMusicPlayer");
        }

        protected override void OnLaunch()
        {
            // OnLaunch is fired after root ViewModel is loaded, so Seer events are fired after views have started.
            BmpSeer.Instance.Start();
            BmpGrunt.Instance.Start();
            BmpMaestro.Instance.Start();
            BmpSiren.Instance.Setup();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LogManager.Shutdown();

            BmpMaestro.Instance.Stop();

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

            Setup(Application.Current);

            _started = true;
            return Task.FromResult(_started);
        }
    }
}