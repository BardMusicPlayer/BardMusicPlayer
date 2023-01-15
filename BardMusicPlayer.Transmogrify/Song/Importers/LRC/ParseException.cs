#region

using System;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.LrcParser;

/// <summary>
///     Exception in parsing.
/// </summary>
public sealed class ParseException : Exception
{
    internal ParseException(string data, int pos, string message, Exception innerException)
        : base(generateMessage(data, pos, message), innerException)
    {
        RawLyrics = data;
        Position = pos;
    }

    /// <summary>
    ///     Position of exception.
    /// </summary>
    public int Position { get; }

    /// <summary>
    ///     Raw lrc data of exception.
    /// </summary>
    public string RawLyrics { get; }

    private static string generateMessage(string data, int pos, string message)
    {
        return $@"{message} Position: {pos}";
    }
}