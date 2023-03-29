/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Transmogrify.Song.Importers.LRC;

/// <summary>
///     Exception in parsing.
/// </summary>
public sealed class ParseException : Exception
{
    internal ParseException(string data, int pos, string message, Exception innerException)
        : base(generateMessage(data, pos, message), innerException)
    {
        RawLyrics = data;
        Position  = pos;
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