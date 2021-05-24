/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Ui.Utilities;
using BardMusicPlayer.Ui.ViewModels.Settings;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class MainViewModel : Conductor<IScreen>
    {
        public MainViewModel(IContainer ioc)
        {
            TopPage  = ioc.Get<TopPageViewModel>();
            Bards    = ioc.Get<BardViewModel>();
            Settings = ioc.Get<SettingsViewModel>();

            ActiveItem = TopPage;
        }

        public IScreen Bards { get; }

        public IScreen Settings { get; }

        public IScreen TopPage { get; }

        public void Navigate(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: IScreen screen }) ActivateItem(screen);
        }

        protected override void OnViewLoaded()
        {
            BmpPigeonhole.Initialize(Globals.DataPath + @"\Configuration.json");

            // var view = (MainView)View;
            // LogManager.Initialize(new(view.Log));

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
    }
}