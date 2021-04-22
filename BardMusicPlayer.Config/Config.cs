namespace BardMusicPlayer.Config
{
    public class Config : JsonSettings.JsonSettings
    {






        public override string FileName { get; set; }
        public Config(string fileName) : base(fileName) { }
    }
}
