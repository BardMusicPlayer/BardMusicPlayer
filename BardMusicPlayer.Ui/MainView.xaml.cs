/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.ComponentModel;
using System.Windows;
using BardMusicPlayer.Catalog;
using BardMusicPlayer.Config;
using BardMusicPlayer.Grunt;
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
            // BmpDoot.Instance.Stop();

            BmpGrunt.Instance.Stop();

            BmpSeer.Instance.Stop();

            BmpCatalog.Instance.Dispose();

            BmpConfig.Instance.Dispose();
        }
    }
}
