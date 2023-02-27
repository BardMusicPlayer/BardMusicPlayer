using System.IO;
using System.Reflection;

namespace BardMusicPlayer.Globals;

public static class Globals
{
    public static string DirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    public static string FileFilters = "All files|*.mid;*.midi;*.mmsong;*.mml;*.gp*|MIDI files|*.mid;*.midi|MMSong files|*.mmsong|MML files|*.mml|GP files|*.gp*";
    public static string MusicCatalogFilters = "Amp Catalog file|*.db";
    public static string DataPath;

    public static event EventHandler OnConfigReload;
    public static void ReloadConfig()
    {
        OnConfigReload?.Invoke(null, null);
    }

}