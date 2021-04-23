using System;
using System.Diagnostics;
using System.Windows;

namespace BardMusicPlayer.Ui
{
    public partial class Main : Window
    {
        private bool _started;
        public bool StartUp(bool beta, int build, string commit, string extraJson, string exePath, string versionPath, string dataPath, string[] args)
        {
            if (_started) throw new Exception("Cannot start up twice.");
            _started = true;
            ShowDialog();
            return true;
        }

        public Main()
        {
            InitializeComponent();
        }
    }
}
