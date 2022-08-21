/*
 * Copyright(c) 2022 MoogleTroupe, isaki, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Coffer
{
    internal static class Constants
    {
        // LiteDB Collection Names
        internal const string PLAYLIST_COL_NAME = "playlist";
        internal const string SONG_COL_NAME = "song";
        internal const string SCHEMA_COL_NAME = "schema";

        // Schema Version
        internal const byte SCHEMA_VERSION = 1;

        // Schema Document ID
        internal const int SCHEMA_DOCUMENT_ID = 1;
    }
}
