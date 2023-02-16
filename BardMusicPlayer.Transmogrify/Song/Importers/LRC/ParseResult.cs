#region

using System.Collections.Generic;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LRC;

/// <summary>
///     Result of parsing.
/// </summary>
/// <typeparam name="TLine">Type of lyrics line.</typeparam>
public interface IParseResult<TLine>
    where TLine : Line
{
    /// <summary>
    ///     Lyrics of parsing result.
    /// </summary>
    Lyrics<TLine> Lyrics { get; }

    /// <summary>
    ///     Exceptions during parsing.
    /// </summary>
    IReadOnlyList<ParseException> Exceptions { get; }
}