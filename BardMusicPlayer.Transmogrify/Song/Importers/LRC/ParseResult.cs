/*
 * Copyright(c) 2018 OpportunityLiu
 * Licensed under Apache License, Version 2.0. See https://raw.githubusercontent.com/OpportunityLiu/LrcParser/master/LICENSE for full license information.
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