/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Collections
{
    internal class StringBuilder
    {
        private readonly System.Text.StringBuilder _sb;

        public StringBuilder()
        {
            _sb = new System.Text.StringBuilder();
        }

        public void AppendChar(int i)
        {
            _sb.Append(Platform.StringFromCharCode(i));
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
