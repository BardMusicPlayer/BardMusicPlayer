/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.IO;
using System.Text;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Notate.Song.Utilities
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static MidiFile ReadAsMidiFile(this Stream stream)
        {
            return MidiFile.Read(stream, new ReadingSettings
            {
                TextEncoding = Encoding.UTF8,
                InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
                InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits,
                InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits,
                InvalidSystemCommonEventParameterValuePolicy = InvalidSystemCommonEventParameterValuePolicy.SnapToLimits,
                MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Ignore,
                NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                UnexpectedTrackChunksCountPolicy = UnexpectedTrackChunksCountPolicy.Ignore,
                UnknownChannelEventPolicy = UnknownChannelEventPolicy.SkipStatusByteAndOneDataByte,
                UnknownChunkIdPolicy = UnknownChunkIdPolicy.ReadAsUnknownChunk
            });
        }
    }
}
