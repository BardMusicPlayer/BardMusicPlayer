/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Siren.AlphaTab.Util
{
    internal class Lazy<T>
    {
        private readonly Func<T> _factory;
        private bool _created;
        private T _value;

        public Lazy(Func<T> factory)
        {
            _factory = factory;
        }

        public T Value
        {
            get
            {
                if (!_created)
                {
                    _value = _factory();
                    _created = true;
                }

                return _value;
            }
        }
    }
}
