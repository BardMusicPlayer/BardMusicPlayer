using Sanford.Multimedia.Midi;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace FFBardMusicPlayer
{
    internal class BmpPluginHelper
    {
        public List<string> LoadedDlls = new List<string>();

        public static BmpPluginHelper LoadPlugins()
        {
            var pluginHelper = new BmpPluginHelper();

            // Search the "Plugins" subdirectory for assemblies that match the imports.
            var catalog = new DirectoryCatalog(".", "FFBmp*.dll");
            using (var container = new CompositionContainer(catalog))
            {
                // Match Imports in "prgm" object with corresponding exports in all catalogs in the container
                container.ComposeParts(pluginHelper);
            }

            return pluginHelper;
        }

        public bool LoadFile(string file, out Sequencer seq, out string error)
        {
            seq   = null;
            error = $"Unknown song filetype. [{Path.GetExtension(file)}]";

            var ext = Path.GetExtension(file).Substring(1).ToLower();

            foreach (var plugin in Plugins)
            {
                if (plugin.GetAssociatedExtension() == ext)
                {
                    seq   = plugin.LoadFile(file);
                    error = plugin.GetResultString();
                    return true;
                }
            }

            return false;
        }

        [ImportMany] private IEnumerable<FFBmpPluginBase> Plugins { get; set; }
    }
}