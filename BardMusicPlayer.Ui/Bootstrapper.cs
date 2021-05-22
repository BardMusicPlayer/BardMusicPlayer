using System;
using System.Threading.Tasks;
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

            Start(args);

            _started = true;
            return Task.FromResult(_started);
        }
    }
}