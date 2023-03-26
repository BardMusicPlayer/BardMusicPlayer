using BardMusicPlayer.DryWetMidi.Common.DataTypes;
using BardMusicPlayer.DryWetMidi.Core.Events.Base;
using BardMusicPlayer.DryWetMidi.Core.Events.Channel;
using BardMusicPlayer.DryWetMidi.Core.Events.Info;
using BardMusicPlayer.DryWetMidi.Core.Exceptions;
using BardMusicPlayer.DryWetMidi.Core.ReadingSettings;

namespace BardMusicPlayer.DryWetMidi.Core.Events.Readers;

internal sealed class ChannelEventReader : IEventReader
{
    #region IEventReader

    public MidiEvent Read(MidiReader reader, ReadingSettings.ReadingSettings settings, byte currentStatusByte)
    {
        var statusByte = currentStatusByte.GetHead();
        var channel = currentStatusByte.GetTail();

        ChannelEvent channelEvent;

        switch (statusByte)
        {
            case EventStatusBytes.Channel.NoteOff:
                channelEvent = new NoteOffEvent();
                break;
            case EventStatusBytes.Channel.NoteOn:
                channelEvent = new NoteOnEvent();
                break;
            case EventStatusBytes.Channel.ControlChange:
                channelEvent = new ControlChangeEvent();
                break;
            case EventStatusBytes.Channel.PitchBend:
                channelEvent = new PitchBendEvent();
                break;
            case EventStatusBytes.Channel.ChannelAftertouch:
                channelEvent = new ChannelAftertouchEvent();
                break;
            case EventStatusBytes.Channel.ProgramChange:
                channelEvent = new ProgramChangeEvent();
                break;
            case EventStatusBytes.Channel.NoteAftertouch:
                channelEvent = new NoteAftertouchEvent();
                break;
            default:
                ReactOnUnknownChannelEvent(statusByte, channel, reader, settings);
                return null;
        }

        channelEvent.Read(reader, settings, MidiEvent.UnknownContentSize);
        channelEvent.Channel = channel;

        if (channelEvent.EventType == MidiEventType.NoteOn)
        {
            var noteOnEvent = (NoteOnEvent)channelEvent;
            if (settings.SilentNoteOnPolicy == SilentNoteOnPolicy.NoteOff && noteOnEvent.Velocity == 0)
                channelEvent = new NoteOffEvent
                {
                    DeltaTime  = noteOnEvent.DeltaTime,
                    Channel    = noteOnEvent.Channel,
                    NoteNumber = noteOnEvent.NoteNumber
                };
        }

        return channelEvent;
    }

    #endregion

    #region Methods

    private void ReactOnUnknownChannelEvent(FourBitNumber statusByte, FourBitNumber channel, MidiReader reader, ReadingSettings.ReadingSettings settings)
    {
        switch (settings.UnknownChannelEventPolicy)
        {
            case UnknownChannelEventPolicy.Abort:
                throw new UnknownChannelEventException(statusByte, channel);
            case UnknownChannelEventPolicy.SkipStatusByte:
                return;
            case UnknownChannelEventPolicy.SkipStatusByteAndOneDataByte:
                reader.Position += 1;
                return;
            case UnknownChannelEventPolicy.SkipStatusByteAndTwoDataBytes:
                reader.Position += 2;
                return;
            case UnknownChannelEventPolicy.UseCallback:
                var callback = settings.UnknownChannelEventCallback;
                if (callback == null)
                    throw new InvalidOperationException("Unknown channel event callback is not set.");

                var action = callback(statusByte, channel);
                switch (action.Instruction)
                {
                    case UnknownChannelEventInstruction.Abort:
                        throw new UnknownChannelEventException(statusByte, channel);
                    case UnknownChannelEventInstruction.SkipData:
                        reader.Position += action.DataBytesToSkipCount;
                        return;
                }
                break;
        }
    }

    #endregion
}