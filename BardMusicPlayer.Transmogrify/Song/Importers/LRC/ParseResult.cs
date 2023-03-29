/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

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