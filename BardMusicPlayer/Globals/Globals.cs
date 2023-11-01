namespace BardMusicPlayer.Globals;

public static class Globals
{
    public static string? DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BMP2";
    public const string FileFilters = "All files|*.mid;*.midi;*.mmsong;*.mml;*.gp*|MIDI files|*.mid;*.midi|MMSong files|*.mmsong|MML files|*.mml|GP files|*.gp*";
    public const string MusicCatalogFilters = "Amp Catalog file|*.db";
    internal static string? DataPath;
}