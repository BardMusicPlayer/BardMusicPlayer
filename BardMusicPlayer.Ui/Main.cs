using System;
using System.Threading.Tasks;
using BardMusicPlayer.Catalog;
using BardMusicPlayer.Config;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Ui
{
    public class Main
    {
        private bool _started;
        public Task<bool> StartUp(bool beta, int build, string commit, string exePath, string versionPath, string dataPath, string[] args)
        {
            if (_started) throw new Exception("Cannot start up twice.");

            BmpConfig.Initialize(@Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BardMusicPlayer\Config.json");

            BmpCatalog.Initialize(@Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BardMusicPlayer\Catalog.litedb");

            BmpSeer.Instance.SetupFirewall("BardMusicPlayer");

            BmpSeer.Instance.Start();

            BmpGrunt.Instance.Start();

            var testWindow = new TestWindow();
            testWindow.ShowDialog();

            _started = true;

            return Task.FromResult(_started);
        }
    }
}
