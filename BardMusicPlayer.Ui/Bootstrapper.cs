using System.Threading.Tasks;
using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Ui.Utilities;
using BardMusicPlayer.Ui.ViewModels;
using Stylet;

namespace BardMusicPlayer.Ui
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {
        private bool _started;

        protected override void OnStart()
        {
            BmpPigeonhole.Initialize(Globals.DataPath + @"\Configuration.json");

            // var view = (MainView)View;
            // LogManager.Initialize(new(view.Log));

            BmpCoffer.Initialize(Globals.DataPath + @"\MusicCatalog.db");

            BmpSeer.Instance.SetupFirewall("BardMusicPlayer");

            BmpSeer.Instance.Start();

            BmpGrunt.Instance.Start();
        }

        protected override void OnExit(ExitEventArgs e)
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

            Setup(Application.Current);

            _started = true;
            return Task.FromResult(_started);
        }
    }
}