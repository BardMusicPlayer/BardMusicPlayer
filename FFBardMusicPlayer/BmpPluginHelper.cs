using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;

namespace FFBardMusicPlayer
{
	class BmpPluginHelper
	{
		public List<string> loadedDlls = new List<string>();
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
			seq = null;
			error = string.Format("Unknown song filetype. [{0}]", Path.GetExtension(file));

			string ext = Path.GetExtension(file).Substring(1).ToLower();

			foreach(FFBmpPluginBase plugin in Plugins) {
				if(plugin.GetAssociatedExtension() == ext)
				{
					seq = plugin.LoadFile(file);
					error = plugin.GetResultString();
					return true;
				}
			}
			return false;

		}

		[ImportMany]
		private IEnumerable<FFBmpPluginBase> Plugins { get; set; }
	}
}
