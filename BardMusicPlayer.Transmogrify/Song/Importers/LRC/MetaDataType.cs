#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LrcParser;

/// <summary>
///     Type of metadata.
/// </summary>
[DebuggerDisplay(@"{" + nameof(Tag) + @"}")]
public abstract class MetaDataType : IEquatable<MetaDataType>
{
    private static readonly char[] invalidChars = "]:".ToCharArray();

    /// <summary>
    ///     Create new instance of <see cref="MetaDataType" />.
    /// </summary>
    /// <param name="tag">Tag of metadata.</param>
    /// <param name="dataType">Data type of metadata.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="tag" /> or <paramref name="dataType" /> is
    ///     <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="tag" /> contains invalid character.</exception>
    protected MetaDataType(string tag, Type dataType)
        : this(tag, dataType, false)
    {
    }

    internal MetaDataType(string tag, Type dataType, bool isSafe)
    {
        if (!isSafe) tag = checkTag(tag);

        DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        Tag = tag;
    }

    /// <summary>
    ///     Tag of metadata.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    ///     Data type of metadata.
    /// </summary>
    public Type DataType { get; }

    /// <summary>
    ///     Default value of metadata.
    /// </summary>
    public virtual object Default => Activator.CreateInstance(DataType);

    /// <inheritdoc />
    public bool Equals(MetaDataType other)
    {
        return Tag.Equals(other?.Tag);
    }

    private static string checkTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentNullException(tag);

        Helper.CheckString(nameof(tag), tag, invalidChars);
        tag = tag.Trim();
        return tag;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Tag;
    }

    /// <summary>
    ///     Parse metadata content string.
    /// </summary>
    /// <param name="mataDataContent">Metadata content string.</param>
    /// <returns>Parsed metadata content, should be of <see cref="DataType" />.</returns>
    protected internal abstract object Parse(string mataDataContent);

    /// <summary>
    ///     Convert metadata content to string.
    /// </summary>
    /// <param name="mataDataContent">Metadata content of <see cref="DataType" />.</param>
    /// <returns>String representation of <paramref name="mataDataContent" />.</returns>
    protected internal virtual string Stringify(object mataDataContent)
    {
        return mataDataContent?.ToString();
    }

    /// <summary>
    ///     Create new instance of <see cref="MetaDataType" />, if <paramref name="tag" /> is known, value from
    ///     <see cref="PreDefined" /> will be returned.
    /// </summary>
    /// <param name="tag">Tag of metadata.</param>
    /// <returns>New instance of <see cref="MetaDataType" />, or instance from <see cref="PreDefined" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tag" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException"><paramref name="tag" /> contains invalid character.</exception>
    public static MetaDataType Create(string tag)
    {
        tag = checkTag(tag);
        return PreDefined.TryGetValue(tag, out var r) ? r : new NoValidateMetaDataType(tag);
    }

    /// <summary>
    ///     Create new instance of <see cref="MetaDataType" />, if <paramref name="tag" /> is known, value from
    ///     <see cref="PreDefined" /> will be returned.
    /// </summary>
    /// <param name="tag">Tag of metadata.</param>
    /// <param name="parser">Parser for <see cref="Parse(string)" /> method.</param>
    /// <param name="defaultValue">Default value of metadata.</param>
    /// <returns>New instance of <see cref="MetaDataType" />, or instance from <see cref="PreDefined" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="tag" /> or <paramref name="parser" /> is
    ///     <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="tag" /> contains invalid character.</exception>
    /// <remarks>
    ///     If instance from <see cref="PreDefined" /> is returned, <paramref name="parser" /> will be ignored.
    /// </remarks>
    public static MetaDataType Create<T>(string tag, Func<string, T> parser, T defaultValue)
    {
        return Create(tag, parser, null, defaultValue);
    }

    /// <summary>
    ///     Create new instance of <see cref="MetaDataType" />, if <paramref name="tag" /> is known, value from
    ///     <see cref="PreDefined" /> will be returned.
    /// </summary>
    /// <param name="tag">Tag of metadata.</param>
    /// <param name="parser">Parser for <see cref="Parse(string)" /> method.</param>
    /// <param name="stringifier">Stringifier for <see cref="Stringify(object)" /> method.</param>
    /// <returns>New instance of <see cref="MetaDataType" />, or instance from <see cref="PreDefined" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="tag" /> or <paramref name="parser" /> is
    ///     <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="tag" /> contains invalid character.</exception>
    /// <remarks>
    ///     If instance from <see cref="PreDefined" /> is returned, <paramref name="parser" /> and
    ///     <paramref name="stringifier" /> will be ignored.
    /// </remarks>
    public static MetaDataType Create<T>(string tag, Func<string, T> parser, Func<T, string> stringifier)
    {
        return Create(tag, parser, stringifier, default);
    }

    /// <summary>
    ///     Create new instance of <see cref="MetaDataType" />, if <paramref name="tag" /> is known, value from
    ///     <see cref="PreDefined" /> will be returned.
    /// </summary>
    /// <param name="tag">Tag of metadata.</param>
    /// <param name="parser">Parser for <see cref="Parse(string)" /> method.</param>
    /// <param name="stringifier">Stringifier for <see cref="Stringify(object)" /> method.</param>
    /// <param name="defaultValue">Default value of metadata.</param>
    /// <returns>New instance of <see cref="MetaDataType" />, or instance from <see cref="PreDefined" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="tag" /> or <paramref name="parser" /> is
    ///     <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="tag" /> contains invalid character.</exception>
    /// <remarks>
    ///     If instance from <see cref="PreDefined" /> is returned, <paramref name="parser" /> and
    ///     <paramref name="stringifier" /> will be ignored.
    /// </remarks>
    public static MetaDataType Create<T>(string tag, Func<string, T> parser, Func<T, string> stringifier = null,
        T defaultValue = default)
    {
        if (parser is null)
            throw new ArgumentNullException(nameof(parser));

        tag = checkTag(tag);
        return PreDefined.TryGetValue(tag, out var r)
            ? r
            : new DelegateMetaDataType<T>(tag, parser, stringifier, defaultValue);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is MetaDataType dt && Equals(dt);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Tag.GetHashCode();
    }

    private sealed class NoValidateMetaDataType : MetaDataType
    {
        public NoValidateMetaDataType(string tag)
            : base(tag, typeof(string), true)
        {
        }

        public override object Default => "";

        protected internal override object Parse(string mataDataContent)
        {
            return mataDataContent;
        }
    }

    private sealed class DelegateMetaDataType<T> : MetaDataType
    {
        private readonly T def;

        private readonly Func<string, T> parser;
        private readonly Func<T, string> stringifier;

        public DelegateMetaDataType(string tag, Func<string, T> parser, Func<T, string> stringifier, T def)
            : base(tag, typeof(T), true)
        {
            this.parser = parser;
            this.stringifier = stringifier;
            this.def = def;
        }

        public override object Default => def;

        protected internal override object Parse(string mataDataContent)
        {
            return parser(mataDataContent);
        }

        protected internal override string Stringify(object mataDataContent)
        {
            if (mataDataContent is T data && stringifier != null)
                return stringifier(data);

            return base.Stringify(mataDataContent);
        }
    }

    #region Pre-defined

    /// <summary>
    ///     Pre-defined <see cref="MetaDataType" />.
    /// </summary>
    public static IReadOnlyDictionary<string, MetaDataType> PreDefined { get; }
        = new ReadOnlyDictionary<string, MetaDataType>(
            new Dictionary<string, MetaDataType>(StringComparer.OrdinalIgnoreCase)
            {
                ["ar"] = new NoValidateMetaDataType("ar"),
                ["al"] = new NoValidateMetaDataType("al"),
                ["ti"] = new NoValidateMetaDataType("ti"),
                ["au"] = new NoValidateMetaDataType("au"),
                ["by"] = new NoValidateMetaDataType("by"),
                ["offset"] = new DelegateMetaDataType<TimeSpan>("offset",
                    static v => TimeSpan.FromTicks((long)(double.Parse(v, NumberStyles.Any) * 10000)),
                    static ts => ts.TotalMilliseconds.ToString("+0.#;-0.#"), default),
                ["re"] = new NoValidateMetaDataType("re"),
                ["ve"] = new NoValidateMetaDataType("ve"),
                ["length"] = new DelegateMetaDataType<DateTime>("length", static v =>
                {
                    if (DateTimeExtension.TryParseLrcString(v, 0, v.Length, out var r))
                        return r;

                    throw new ArgumentException("Invalid length string.");
                }, static d => d.ToTimestamp().ToLrcStringShort(), default)
            });

    /// <summary>
    ///     Lyrics artist, "ar" field of ID Tags.
    /// </summary>
    public static MetaDataType Artist => PreDefined["ar"];

    /// <summary>
    ///     Album where the song is from, "al" field of ID Tags.
    /// </summary>
    public static MetaDataType Album => PreDefined["al"];

    /// <summary>
    ///     Lyrics(song) title, "ti" field of ID Tags.
    /// </summary>
    public static MetaDataType Title => PreDefined["ti"];

    /// <summary>
    ///     Creator of the songtext, "au" field of ID Tags.
    /// </summary>
    public static MetaDataType Author => PreDefined["au"];

    /// <summary>
    ///     Creator of the LRC file, "by" field of ID Tags.
    /// </summary>
    public static MetaDataType Creator => PreDefined["by"];

    /// <summary>
    ///     Overall timestamp adjustment, "offset" field of ID Tags.
    /// </summary>
    public static MetaDataType Offset => PreDefined["offset"];

    /// <summary>
    ///     The player or editor that created the LRC file, "re" field of ID Tags.
    /// </summary>
    public static MetaDataType Editor => PreDefined["re"];

    /// <summary>
    ///     Version of program, "ve" field of ID Tags.
    /// </summary>
    public static MetaDataType Version => PreDefined["ve"];

    /// <summary>
    ///     Length of song, "length" field of ID Tags.
    /// </summary>
    public static MetaDataType Length => PreDefined["length"];

    #endregion Pre-defined
}