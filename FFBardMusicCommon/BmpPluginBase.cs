using System;
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

namespace FFBardMusicPlayer
{
	public interface FFBmpPluginBase
	{
		Sequencer LoadFile(string filename);

		string GetResultString();

		string GetAssociatedExtension();
	}
}
