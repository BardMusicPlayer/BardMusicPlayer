﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LRC;

/// <summary>
///     Collection of <see cref="Line" />.
/// </summary>
/// <typeparam name="TLine">Type of lyrics line.</typeparam>
public sealed class LineCollection<TLine> : List<TLine>
    where TLine : Line
{
    internal LineCollection() : base(25)
    {
    }

    /// <summary>
    ///     Apply <paramref name="offset" /> to items in the <see cref="LineCollection{TLine}" />.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset" /> out of range for some line.</exception>
    public void ApplyOffset(TimeSpan offset)
    {
        if (offset == default)
            return;

        var i = 0;
        try
        {
            for (; i < Count; i++)
            {
                var line = this[i];
                line.InternalTimestamp += offset;
            }
        }
        catch
        {
            for (var j = 0; j < i; j++)
            {
                var line = this[j];
                line.InternalTimestamp -= offset;
            }

            throw;
        }
    }

    internal StringBuilder ToString(StringBuilder sb, LyricsFormat format)
    {
        var datasource = this.AsEnumerable();
        if (format.Flag(LyricsFormat.LinesSortByContent)) datasource = datasource.OrderBy(static l => l.Content);

        if (format.Flag(LyricsFormat.LinesSortByTimestamp))
            datasource = datasource is IOrderedEnumerable<TLine> od
                ? od.ThenBy(static l => l.InternalTimestamp)
                : datasource.OrderBy(static l => l.InternalTimestamp);

        if (format.Flag(LyricsFormat.MergeTimestamp))
            foreach (var item in datasource.GroupBy(static l => l.Content))
            {
                foreach (var line in item) line.TimestampToString(sb);
                sb.AppendLine(item.Key);
            }
        else
            foreach (var item in datasource)
                item.ToString(sb).AppendLine();

        return sb;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder(Count * 20);
        ToString(sb, LyricsFormat.Default);
        return sb.ToString();
    }
}