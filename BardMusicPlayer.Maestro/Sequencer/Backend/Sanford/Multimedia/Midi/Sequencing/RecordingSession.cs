using System.Collections.Generic;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Clocks;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing.TrackClasses;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing;

public class RecordingSession
{
    private IClock clock;

    private List<TimestampedMessage> buffer = new List<TimestampedMessage>();

    private Track result = new Track();

    public RecordingSession(IClock clock)
    {
        this.clock = clock;
    }

    public void Build()
    {
        result = new Track();

        buffer.Sort(new TimestampComparer());

        foreach (var tm in buffer)
        {
            result.Insert(tm.ticks, tm.message);
        }
    }

    public void Clear()
    {
        buffer.Clear();
    }

    public Track Result
    {
        get
        {
            return result;
        }
    }

    public void Record(ChannelMessage message)
    {
        if (clock.IsRunning)
        {
            buffer.Add(new TimestampedMessage(clock.Ticks, message));
        }
    }

    public void Record(SysExMessage message)
    {
        if (clock.IsRunning)
        {
            buffer.Add(new TimestampedMessage(clock.Ticks, message));
        }
    }

    private struct TimestampedMessage
    {
        public int ticks;

        public IMidiMessage message;

        public TimestampedMessage(int ticks, IMidiMessage message)
        {
            this.ticks   = ticks;
            this.message = message;
        }
    }

    private class TimestampComparer : IComparer<TimestampedMessage>
    {
        #region IComparer<TimestampedMessage> Members

        public int Compare(TimestampedMessage x, TimestampedMessage y)
        {
            if (x.ticks > y.ticks)
            {
                return 1;
            }
            else if (x.ticks < y.ticks)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}