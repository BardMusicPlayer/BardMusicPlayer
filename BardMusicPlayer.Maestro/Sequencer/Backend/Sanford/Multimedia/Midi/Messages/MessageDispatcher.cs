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
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages.EventArg;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing.TrackClasses;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages
{
    /// <summary>
    /// Dispatches IMidiMessages to their corresponding sink.
    /// </summary>
    public class MessageDispatcher
    {
        #region MessageDispatcher Members

        #region Events

        public event EventHandler<ChannelMessageEventArgs> ChannelMessageDispatched;

        public event EventHandler<SysExMessageEventArgs> SysExMessageDispatched;

        public event EventHandler<SysCommonMessageEventArgs> SysCommonMessageDispatched;

        public event EventHandler<SysRealtimeMessageEventArgs> SysRealtimeMessageDispatched;

        public event EventHandler<MetaMessageEventArgs> MetaMessageDispatched;

        #endregion

        /// <summary>
        /// Dispatches IMidiMessages to their corresponding sink.
        /// </summary>
        /// <param name="message">
        /// The IMidiMessage to dispatch.
        /// </param>
        public void Dispatch(Track track, IMidiMessage message)
        {
            #region Require

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            #endregion

            switch (message.MessageType)
            {
                case MessageType.Channel:
                    OnChannelMessageDispatched(new ChannelMessageEventArgs(track, (ChannelMessage)message));
                    break;

                case MessageType.SystemExclusive:
                    OnSysExMessageDispatched(new SysExMessageEventArgs(track, (SysExMessage)message));
                    break;

                case MessageType.Meta:
                    OnMetaMessageDispatched(new MetaMessageEventArgs(track, (MetaMessage)message));
                    break;

                case MessageType.SystemCommon:
                    OnSysCommonMessageDispatched(new SysCommonMessageEventArgs((SysCommonMessage)message));
                    break;

                case MessageType.SystemRealtime:
                    switch (((SysRealtimeMessage)message).SysRealtimeType)
                    {
                        case SysRealtimeType.ActiveSense:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.ActiveSense);
                            break;

                        case SysRealtimeType.Clock:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Clock);
                            break;

                        case SysRealtimeType.Continue:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Continue);
                            break;

                        case SysRealtimeType.Reset:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Reset);
                            break;

                        case SysRealtimeType.Start:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Start);
                            break;

                        case SysRealtimeType.Stop:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Stop);
                            break;

                        case SysRealtimeType.Tick:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Tick);
                            break;
                    }

                    break;
            }
        }

        protected virtual void OnChannelMessageDispatched(ChannelMessageEventArgs e)
        {
            var handler = ChannelMessageDispatched;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSysExMessageDispatched(SysExMessageEventArgs e)
        {
            var handler = SysExMessageDispatched;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSysCommonMessageDispatched(SysCommonMessageEventArgs e)
        {
            var handler = SysCommonMessageDispatched;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs e)
        {
            var handler = SysRealtimeMessageDispatched;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnMetaMessageDispatched(MetaMessageEventArgs e)
        {
            var handler = MetaMessageDispatched;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}