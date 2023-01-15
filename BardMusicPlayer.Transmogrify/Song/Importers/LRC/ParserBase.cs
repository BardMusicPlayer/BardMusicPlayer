#region

using System.Collections.Generic;
using System.Threading;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LrcParser;

internal abstract class ParserBase<TLine> : IParseResult<TLine>
    where TLine : Line
{
    protected static readonly char[] LINE_BREAKS = "\r\n\u0085\u2028\u2029".ToCharArray();

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