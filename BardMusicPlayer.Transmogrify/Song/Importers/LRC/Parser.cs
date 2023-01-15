#region

using System;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LrcParser;

internal sealed class Parser<TLine> : ParserBase<TLine>
    where TLine : Line, new()
{
    private int currentPosition;

    public Parser(string data) : base(data)
    {
    }

    private void skipWhitespaces()
    {
        for (; currentPosition < Data.Length; currentPosition++)
            if (!char.IsWhiteSpace(Data[currentPosition]))
                break;
    }

    private void trimEnd(ref int end)
    {
        for (var i = end - 1; i >= currentPosition; i--)
            if (!char.IsWhiteSpace(Data[i]))
            {
                end = i + 1;
                break;
            }
    }

    private int readLine()
    {
        if (currentPosition >= Data.Length)
            return -1;

        currentPosition = Data.IndexOf('[', currentPosition);
        if (currentPosition < 0)
        {
            currentPosition = Data.Length;
            return -1;
        }

        var nextPosition = currentPosition + 1;
        if (nextPosition >= Data.Length)
            return nextPosition;

        nextPosition = Data.IndexOfAny(LINE_BREAKS, nextPosition);
        if (nextPosition < 0) return Data.Length;

        nextPosition = Data.IndexOf('[', nextPosition);
        return nextPosition < 0 ? Data.Length : nextPosition;
    }

    private bool readTag(int next, out int tagStart, out int tagEnd)
    {
        tagStart = -1;
        tagEnd = -1;
        skipWhitespaces();
        var lbPos = currentPosition;
        if (lbPos >= next)
            return false; // empty range
        if (Data[lbPos] != '[')
            return false; // not a tag

        currentPosition++;
        skipWhitespaces();
        if (currentPosition >= next)
        {
            currentPosition = lbPos;
            return false; // ']' not found
        }

        tagStart = currentPosition;
        var rbPos = default(int);
        // timestamp
        rbPos = char.IsDigit(Data[tagStart])
            ? Data.IndexOf(']', tagStart, next - tagStart)
            :
            // ID tag
            Data.LastIndexOf(']', next - 1, next - tagStart);
        if (rbPos < 0)
        {
            currentPosition = lbPos;
            return false; // ']' not found
        }

        tagEnd = rbPos;
        trimEnd(ref tagEnd);
        currentPosition = rbPos + 1;
        return true;
    }

    private void analyzeLine(int next)
    {
        var current = currentPosition;
        var lineStart = Lines.Count;
        var isIdTagLine = true;
        // analyze tag of line
        while (true)
        {
            var oldPos = currentPosition;
            if (!readTag(next, out var tagStart, out var tagEnd))
                break;

            if (DateTimeExtension.TryParseLrcString(Data, tagStart, tagEnd, out var time))
            {
                Lines.Add(new TLine { InternalTimestamp = time });
                isIdTagLine = false;
                continue;
            }

            if (!isIdTagLine) // not a tag, id tag will not appear after a timestamp
            {
                currentPosition = oldPos;
                break;
            }

            var colum = Data.IndexOf(':', tagStart, tagEnd - tagStart);
            var mdt = colum < 0
                ? MetaDataType.Create(Data.Substring(tagStart, tagEnd - tagStart))
                : MetaDataType.Create(Data.Substring(tagStart, colum - tagStart));
            var mdc = colum < 0
                ? ""
                : Data.Substring(colum + 1, tagEnd - colum - 1);
            try
            {
                MetaData[mdt] = mdt.Stringify(mdt.Parse(mdc));
            }
            catch (Exception ex)
            {
                MetaData[mdt] = mdc;
                Exceptions.Add(new ParseException(Data, colum < 0 ? tagStart : colum + 1,
                    $"Failed to parse ID tag `{mdt}`", ex));
            }
        }

        // analyze content of line
        if (Lines.Count != lineStart)
        {
            skipWhitespaces();
            var end = next;
            trimEnd(ref end);
            var content = Data.Substring(currentPosition, end - currentPosition);
            for (var i = lineStart; i < Lines.Count; i++) Lines[i].Content = content;
        }

        currentPosition = next;
    }

    public void Analyze()
    {
        while (true)
        {
            var nextPosition = readLine();
            if (nextPosition < 0)
                return;

            analyzeLine(nextPosition);
        }
    }
}