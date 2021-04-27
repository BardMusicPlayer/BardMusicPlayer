using BardMusicPlayer.Common;
using System;

namespace BardMusicPlayer.Catalog
{
    public class CatalogException : BmpException
    {
        public CatalogException(string message) : base(message) { }

        public CatalogException(string message, Exception inner) : base(message, inner) { }
    }
}
