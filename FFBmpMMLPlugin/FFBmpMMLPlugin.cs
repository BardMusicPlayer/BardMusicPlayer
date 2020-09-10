using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using FFBardMusicCommon;
using FFBardMusicPlayer;
using Sanford.Multimedia.Midi;

namespace FFBmpMMLPlugin
{
	[Export(typeof(FFBmpPluginBase))]
	public class FFBmpMMLPlugin : FFBmpPluginBase
	{
		string error = "";

		public Sequencer LoadFile(string filename)
		{
			error = "[DUMMY] Plugin response.";
			return new Sequencer();
		}

		public string GetResultString()
		{
			return error;
		}

		public string GetAssociatedExtension()
		{
			return "mml";
		}
	}
}