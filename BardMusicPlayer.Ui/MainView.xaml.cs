/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.ComponentModel;
using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Ui
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MainView_OnClosing(object _, CancelEventArgs e)
        {
            // BmpMaestro.Instance.Stop();

            BmpGrunt.Instance.Stop();

            BmpSeer.Instance.Stop();

            BmpCoffer.Instance.Dispose();

            BmpPigeonhole.Instance.Dispose();
        }
    }
}
