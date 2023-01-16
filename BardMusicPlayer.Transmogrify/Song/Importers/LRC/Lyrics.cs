#region

using System;
using System.Diagnostics;
using System.Text;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LrcParser;

/// <summary>
///     Factory class for <see cref="Lyrics{TLine}" />.
/// </summary>
public static class Lyrics
{
    /// <summary>
    ///     Parse lrc file.
    /// </summary>
    /// <param name="content">Content of lrc file.</param>
    /// <returns>Result of parsing.</returns>
    /// <typeparam name="TLine">Type of lyrics line.</typeparam>
    public static IParseResult<TLine> Parse<TLine>(string content)
        where TLine : Line, new()
    {
        var parser = new Parser<TLine>(content);
        parser.Analyze();
        return parser;
    }

    /// <summary>
    ///     Parse lrc file.
    /// </summary>
    /// <param name="content">Content of lrc file.</param>
    /// <returns>Result of parsing.</returns>
    public static IParseResult<Line> Parse(string content)
    {
        return Parse<Line>(content);
    }

    /// <summary>
    ///     Parse lrc file.
    /// </summary>
    /// <param name="content">Content of lrc file.</param>
    /// <returns>Result of parsing.</returns>
    public static IParseResult<LineWithSpeaker> ParseWithSpeaker(string content)
    {
        return Parse<LineWithSpeaker>(content);
    }
}

/// <summary>
///     Represents lrc file.
/// </summary>
/// <typeparam name="TLine">Type of lyrics line.</typeparam>
[DebuggerDisplay(@"MetaDataCount = {MetaData.Count} LineCount = {Lines.Count}")]
public sealed class Lyrics<TLine>
    where TLine : Line
{
    /// <summary>
    ///     Create new instance of <see cref="Lyrics" />.
    /// </summary>
    public Lyrics()
    {
        Lines = new LineCollection<TLine>();
        MetaData = new MetaDataDictionary();
    }

    internal Lyrics(ParserBase<TLine> parser)
    {
        Lines = parser.Lines;
        MetaData = parser.MetaData;
    }

    /// <summary>
    ///     Content of lyrics.
    /// </summary>
    public LineCollection<TLine> Lines { get; }

    /// <summary>
    ///     Metadata of lyrics.
    /// </summary>
    public MetaDataDictionary MetaData { get; }

    /// <summary>
    ///     Apply <see cref="MetaDataDictionary.Offset" /> to <see cref="Lines" />, then set
    ///     <see cref="MetaDataDictionary.Offset" /> to 0.
    /// </summary>
    /// <exception cref="InvalidOperationException"><see cref="MetaDataDictionary.Offset" /> out of range for some line.</exception>
    public void PreApplyOffset()
    {
        try
        {
            var offset = MetaData.Offset;
            Lines.ApplyOffset(offset);
            MetaData.Offset = default;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new InvalidOperationException("Invalid offset.", ex);
        }
    }

    /// <summary>
    ///     Serialize the lrc file.
    /// </summary>
    /// <param name="format">Format settings for serialization.</param>
    /// <returns>Lrc file data.</returns>
    public string ToString(LyricsFormat format)
    {
        var sb = new StringBuilder(MetaData.Count * 10 + Lines.Count * 20);
        if (format.Flag(LyricsFormat.NewLineAtBeginOfFile))
            sb.AppendLine();
        MetaData.ToString(sb, format);
        if (format.Flag(LyricsFormat.NewLineAtEndOfMetadata))
            sb.AppendLine();
        Lines.ToString(sb, format);
        if (!format.Flag(LyricsFormat.NewLineAtEndOfFile))
            sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        return sb.ToString();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToString(LyricsFormat.Default);
    }
}