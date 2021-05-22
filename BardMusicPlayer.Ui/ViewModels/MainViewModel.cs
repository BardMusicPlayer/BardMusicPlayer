/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Coffer;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Ui.Utilities;
using BardMusicPlayer.Ui.Views;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class MainViewModel : Screen
    {
        public BindableCollection<IGameInformation> Bards { get; } = new();

        protected override void OnViewLoaded()
        {
            var view = (MainView) View;
            BmpPigeonhole.Initialize(Globals.DataPath + @"\Configuration.json");

            LogManager.Initialize(new(view.Log));

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