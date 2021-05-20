/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.ComponentModel;
using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Ui.Controls;
using BardMusicPlayer.Ui.Utilities;

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
            
            BmpPigeonhole.Initialize(@Globals.DataPath + @"\Configuration.json");

            LogManager.Initialize(new LogTextWriter(Log));

            BmpCoffer.Initialize(@Globals.DataPath + @"\MusicCatalog.db");

            BmpSeer.Instance.SetupFirewall("BardMusicPlayer");

            BmpSeer.Instance.Start();

            BmpGrunt.Instance.Start();

            // BmpMaestro.Instance.Start();

        }

        private void MainView_OnClosing(object _, CancelEventArgs e)
        {
            LogManager.Shutdown();

            // BmpMaestro.Instance.Stop();

            BmpGrunt.Instance.Stop();

            BmpSeer.Instance.Stop();

            BmpSeer.Instance.DestroyFirewall("BardMusicPlayer");

            BmpCoffer.Instance.Dispose();

            BmpPigeonhole.Instance.Dispose();
        }
    }
}
