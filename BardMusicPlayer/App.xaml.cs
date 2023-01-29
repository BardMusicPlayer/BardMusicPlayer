using System.Diagnostics;
using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Jamboree;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Script;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Siren;

namespace BardMusicPlayer
{
    /// <summary>
    /// Interaction logic for "App.xaml"
    /// </summary>
    public partial class App
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            Globals.Globals.DataPath = @"data\";

            //init pigeon at first
            BmpPigeonhole.Initialize(Globals.Globals.DataPath + @"\Configuration.json");

            // var view = (MainView)View;
            // LogManager.Initialize(new(view.Log));

            //Load the last used catalog
            var CatalogFile = BmpPigeonhole.Instance.LastLoadedCatalog;
            if (System.IO.File.Exists(CatalogFile))
                BmpCoffer.Initialize(CatalogFile);
            else
                BmpCoffer.Initialize(Globals.Globals.DataPath + @"\MusicCatalog.db");

            //Setup seer
            BmpSeer.Instance.SetupFirewall("BardMusicPlayer");
            //Start maestro before seer, else we'll not get all the players
            BmpMaestro.Instance.Start();
            //Start seer
            BmpSeer.Instance.Start();

            DalamudBridge.DalamudBridge.Instance.Start();

            //Start the scripting
            BmpScript.Instance.Start();

            BmpSiren.Instance.Setup();
            //BmpJamboree.Instance.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //LogManager.Shutdown();
            BmpJamboree.Instance.Stop();
            if (BmpSiren.Instance.IsReadyForPlayback)
                BmpSiren.Instance.Stop();
            BmpSiren.Instance.ShutDown();
            BmpMaestro.Instance.Stop();

            BmpScript.Instance.Stop();

            DalamudBridge.DalamudBridge.Instance.Stop();
            BmpSeer.Instance.Stop();
            BmpSeer.Instance.DestroyFirewall("BardMusicPlayer");
            BmpCoffer.Instance.Dispose();
            BmpPigeonhole.Instance.Dispose();

            //Wasabi hangs kill it with fire
            Process.GetCurrentProcess().Kill();
        }
    }
}
