/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Common;
using System;

namespace BardMusicPlayer.Catalog
{
    public class BmpCatalogException : BmpException
    {
        public BmpCatalogException(string message) : base(message) { }

        public BmpCatalogException(string message, Exception inner) : base(message, inner) { }
    }
}
