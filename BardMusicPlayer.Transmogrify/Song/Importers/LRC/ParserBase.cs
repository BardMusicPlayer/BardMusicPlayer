/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Transmogrify.Song.Importers.LRC;

internal abstract class ParserBase<TLine> : IParseResult<TLine>
    where TLine : Line
{
    protected readonly char[] LINE_BREAKS = "\r\n\u0085\u2028\u2029".ToCharArray();

    protected readonly string Data;
    public readonly LineCollection<TLine> Lines = new();

    public readonly MetaDataDictionary MetaData = new();

    protected List<ParseException> Exceptions = new();

    private Lyrics<TLine> lyrics;

    protected ParserBase(string data)
    {
        Data = data ?? "";
    }

    Lyrics<TLine> IParseResult<TLine>.Lyrics =>
        LazyInitializer.EnsureInitialized(ref lyrics, () => new Lyrics<TLine>(this));

    IReadOnlyList<ParseException> IParseResult<TLine>.Exceptions => Exceptions;
}