#region

using System;
using System.Diagnostics;
using System.Text;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LrcParser;

/// <summary>
///     Represents single line of lyrics.
///     Format: <c>"[mm:ss.ff]Lyrics"</c>
/// </summary>
[DebuggerDisplay(@"{ToString(),nq}")]
public class Line : IComparable<Line>, IComparable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string content = "";

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal DateTime InternalTimestamp;

    /// <summary>
    ///     Create new instance of <see cref="Line" />.
    /// </summary>
    public Line()
    {
    }

    /// <summary>
    ///     Create new instance of <see cref="Line" />.
    /// </summary>
    /// <param name="timestamp">Timestamp of this line.</param>
    /// <param name="content">Lyrics of this line.</param>
    public Line(DateTime timestamp, string content)
    {
        Timestamp = timestamp;
        this.content = (content ?? "").Trim();
    }

    /// <summary>
    ///     Timestamp of this line of lyrics.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     <see cref="DateTime.Kind" /> of value is not
    ///     <see cref="DateTimeKind.Unspecified" />.
    /// </exception>
    public DateTime Timestamp
    {
        get => InternalTimestamp;
        set => InternalTimestamp = value.ToTimestamp();
    }

    /// <summary>
    ///     Lyrics of this line.
    /// </summary>
    public virtual string Content
    {
        get => content;
        set => content = (value ?? "").Trim();
    }

    int IComparable.CompareTo(object obj)
    {
        return CompareTo((Line)obj);
    }

    /// <inheritdoc />
    public int CompareTo(Line other)
    {
        if (other is null)
            return 1;

        var ct = InternalTimestamp.CompareTo(other.InternalTimestamp);
        return ct != 0 ? ct : string.CompareOrdinal(Content, other.Content);
    }

    internal StringBuilder ToString(StringBuilder sb)
    {
        return TimestampToString(sb).Append(Content);
    }

    internal StringBuilder TimestampToString(StringBuilder sb)
    {
        return sb.Append('[')
            .Append(InternalTimestamp.ToLrcString())
            .Append(']');
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToString(new StringBuilder(content.Length + 10)).ToString();
    }
}

/// <summary>
///     Represents single line of lyrics with specified speaker.
///     Format: <c>"[mm:ss.ff]Spearker: Lyrics"</c>
/// </summary>
public sealed class LineWithSpeaker : Line
{
    private static readonly char[] invalidSpeakerChars = ":".ToCharArray();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string speaker = "";

    /// <summary>
    ///     Create new instance of <see cref="Line" />.
    /// </summary>
    public LineWithSpeaker()
    {
    }

    /// <summary>
    ///     Create new instance of <see cref="Line" />.
    /// </summary>
    /// <param name="timestamp">Timestamp of this line.</param>
    /// <param name="speaker">Speaker of this line.</param>
    /// <param name="lyrics">Lyrics of this line.</param>
    public LineWithSpeaker(DateTime timestamp, string speaker, string lyrics)
        : base(timestamp, null)
    {
        Speaker = speaker;
        Lyrics = lyrics;
    }

    /// <summary>
    ///     Speaker of this line.
    /// </summary>
    public string Speaker
    {
        get => speaker;
        set
        {
            value ??= "";
            Helper.CheckString(nameof(value), value, invalidSpeakerChars);
            speaker = value.Trim();
        }
    }

    /// <summary>
    ///     Lyrics of this line.
    /// </summary>
    public string Lyrics
    {
        get => base.Content;
        set => base.Content = value;
    }

    /// <summary>
    ///     Lyrics with speaker of this line.
    /// </summary>
    public override string Content
    {
        get
        {
            if (string.IsNullOrEmpty(speaker))
                return Lyrics;
            if (string.IsNullOrEmpty(Lyrics))
                return Speaker + ":";

            return Speaker + ": " + Lyrics;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                speaker = "";
                Lyrics = "";
                return;
            }

            var pi = value.IndexOf(':');
            if (pi < 0)
            {
                speaker = "";
                Lyrics = value;
            }
            else
            {
                Speaker = value.Substring(0, pi);
                Lyrics = value.Substring(pi + 1);
            }
        }
    }
}