using System.Collections.Generic;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.SongEditor
{
    public class ConfigContainerViewModel : Screen
    {
        private uint _bards;

        public ConfigContainerViewModel(KeyValuePair<long, ConfigContainer> container, string name)
        {
            ConfigurationId = container.Key;
            Container       = container.Value;

            Name = name;
        }

        public bool CanMinusBard => Bards > 1;

        public ConfigContainer Container { get; set; }

        public Instrument? Instrument { get; set; }

        public IProcessorConfig Processor { get; set; }

        public long ConfigurationId { get; set; }

        public OctaveRange? OctaveRange { get; set; }

        public string Name { get; set; }

        public uint Bards { get; set; }

        public void AddBard() { Bards++; }

        public void MinusBard() { Bards--; }
    }
}