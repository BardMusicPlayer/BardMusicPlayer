using System;
using System.Threading.Tasks;

namespace BardMusicPlayer.Ui
{
    public class Main
    {
        private bool _started;
        public async Task<bool> StartUp(bool beta, int build, string commit, string exePath, string versionPath, string dataPath, string[] args)
        {
            if (_started) throw new Exception("Cannot start up twice.");
            _started = true;

            var testWindow = new TestWindow();
            testWindow.ShowDialog();

            return true;
        }
    }
}
