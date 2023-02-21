#region License

/* Copyright (c) 2006 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;
using System.Collections;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages.MessageBuilders;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Processing;

public class ChannelStopper
{
    private ChannelMessage[,] noteOnMessage;

    private bool[] holdPedal1Message;

    private bool[] holdPedal2Message;

    private bool[] sustenutoMessage;

    private ChannelMessageBuilder builder = new ChannelMessageBuilder();

    public event EventHandler<StoppedEventArgs> Stopped;

    public ChannelStopper()
    {
        var c = ChannelMessage.MidiChannelMaxValue + 1;
        var d = ShortMessage.DataMaxValue + 1;

        noteOnMessage = new ChannelMessage[c, d];

        holdPedal1Message = new bool[c];
        holdPedal2Message = new bool[c];
        sustenutoMessage  = new bool[c];
    }

    public void Process(ChannelMessage message)
    {
        switch (message.Command)
        {
            case ChannelCommand.NoteOn:
                if (message.Data2 > 0)
                {
                    noteOnMessage[message.MidiChannel, message.Data1] = message;
                }
                else
                {
                    noteOnMessage[message.MidiChannel, message.Data1] = null;
                }
                break;

            case ChannelCommand.NoteOff:
                noteOnMessage[message.MidiChannel, message.Data1] = null;
                break;

            case ChannelCommand.Controller:
                switch (message.Data1)
                {
                    case (int)ControllerType.HoldPedal1:
                        if (message.Data2 > 63)
                        {
                            holdPedal1Message[message.MidiChannel] = true;
                        }
                        else
                        {
                            holdPedal1Message[message.MidiChannel] = false;
                        }
                        break;

                    case (int)ControllerType.HoldPedal2:
                        if (message.Data2 > 63)
                        {
                            holdPedal2Message[message.MidiChannel] = true;
                        }
                        else
                        {
                            holdPedal2Message[message.MidiChannel] = false;
                        }
                        break;

                    case (int)ControllerType.SustenutoPedal:
                        if (message.Data2 > 63)
                        {
                            sustenutoMessage[message.MidiChannel] = true;
                        }
                        else
                        {
                            sustenutoMessage[message.MidiChannel] = false;
                        }
                        break;
                }
                break;
        }
    }

    public void AllSoundOff()
    {
        var stoppedMessages = new ArrayList();

        for (var c = 0; c <= ChannelMessage.MidiChannelMaxValue; c++)
        {
            for (var n = 0; n <= ShortMessage.DataMaxValue; n++)
            {
                if (noteOnMessage[c, n] != null)
                {
                    builder.MidiChannel = c;
                    builder.Command     = ChannelCommand.NoteOff;
                    builder.Data1       = noteOnMessage[c, n].Data1;
                    builder.Build();

                    stoppedMessages.Add(builder.Result);

                    noteOnMessage[c, n] = null;
                }
            }

            if (holdPedal1Message[c])
            {
                builder.MidiChannel = c;
                builder.Command     = ChannelCommand.Controller;
                builder.Data1       = (int)ControllerType.HoldPedal1;
                builder.Build();

                stoppedMessages.Add(builder.Result);

                holdPedal1Message[c] = false;
            }

            if (holdPedal2Message[c])
            {
                builder.MidiChannel = c;
                builder.Command     = ChannelCommand.Controller;
                builder.Data1       = (int)ControllerType.HoldPedal2;
                builder.Build();

                stoppedMessages.Add(builder.Result);

                holdPedal2Message[c] = false;
            }

            if (sustenutoMessage[c])
            {
                builder.MidiChannel = c;
                builder.Command     = ChannelCommand.Controller;
                builder.Data1       = (int)ControllerType.SustenutoPedal;
                builder.Build();

                stoppedMessages.Add(builder.Result);

                sustenutoMessage[c] = false;
            }
        }

        OnStopped(new StoppedEventArgs(stoppedMessages));
    }

    public void Reset()
    {
        for (var c = 0; c <= ChannelMessage.MidiChannelMaxValue; c++)
        {
            for (var n = 0; n <= ShortMessage.DataMaxValue; n++)
            {
                noteOnMessage[c, n] = null;
            }

            holdPedal1Message[c] = false;
            holdPedal2Message[c] = false;
            sustenutoMessage[c]  = false;
        }
    }

    protected virtual void OnStopped(StoppedEventArgs e)
    {
        var handler = Stopped;

        if (handler != null)
        {
            handler(this, e);
        }
    }
}