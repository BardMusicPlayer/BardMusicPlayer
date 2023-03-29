/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Script.BasicSharp;

public struct Marker
{
    public int Pointer { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public Marker(int pointer, int line, int column)
        : this()
    {
        Pointer = pointer;
        Line    = line;
        Column  = Column;
    }
}