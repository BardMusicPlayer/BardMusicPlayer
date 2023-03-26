/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing.TrackClasses;

namespace BardMusicPlayer.Maestro.Old.Utils;

public class NoteEvent
{
    public Track track;
    public int trackNum;
    public int note;
    public int origNote;
}
public class ProgChangeEvent
{
    public Track track;
    public int trackNum;
    public int voice;
}

public class ChannelAfterTouchEvent
{
    public Track track;
    public int trackNum;
    public int command;
}

public static class NoteHelper
{
    public static int ApplyOctaveShift(int note, int octave)
    {
        return note - 12 * 4 + 12 * octave;
    }
}