using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Ui.Globals
{
    public static class Globals
    {
        public static string DirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string FileFilters = "All files|*.mid;*.midi;*.mmsong;*.mml;*.gp*|MIDI files|*.mid;*.midi|MMSong files|*.mmsong|MML files|*.mml|GP files|*.gp*";
        public static string MusicCatalogFilters = "Amp Catalog file|*.db";
        public static string DataPath;
        public enum Autostart_Types
        {
            NONE = 0,
            VIA_METRONOME,
        }

        public static event EventHandler OnConfigReload;
        public static void ReloadConfig()
        {
            OnConfigReload?.Invoke(null, null);
        }

    }
}
