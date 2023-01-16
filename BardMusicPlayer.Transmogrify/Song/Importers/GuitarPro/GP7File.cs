namespace BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro;

public sealed class GP7File : GPFile
{
    private static string xml;
    //public List<Track> tracks;

    public GP7File(string data)
    {
        GPBase.pointer = 0;
        xml = data;
    }

    public override void readSong()
    {
        var parsedXml = GP6File.ParseGP6(xml, 3);
        var gp5file = GP6File.GP6NodeToGP5File(parsedXml.subnodes[0]);
        tracks = gp5file.tracks;
        self = gp5file;
        self.versionTuple = new[] { 7, 0 };
    }
}