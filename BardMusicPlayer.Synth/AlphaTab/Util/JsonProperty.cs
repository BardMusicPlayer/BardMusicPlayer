using System;

namespace BardMusicPlayer.Synth.AlphaTab.Util
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
