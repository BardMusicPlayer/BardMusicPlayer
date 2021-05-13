/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Siren.AlphaTab.Util
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    internal sealed class JsonSerializableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class JsonImmutableAttribute : Attribute
    {
    }

    [AttributeUsage(System.AttributeTargets.Property)]
    internal sealed class JsonNameAttribute : Attribute
    {
        public string[] Names { get; }

        public JsonNameAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
