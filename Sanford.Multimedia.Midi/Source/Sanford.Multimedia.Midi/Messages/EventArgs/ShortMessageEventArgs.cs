using System;

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    /// Raw short message as int or byte array, useful when working with VST.
    /// </summary>
    public class ShortMessageEventArgs : EventArgs
    {
        ShortMessage message;

        public ShortMessageEventArgs(ShortMessage message)
        {
            this.message = message;
        }

        public ShortMessageEventArgs(int message, int timestamp = 0)
        {
            this.message = new ShortMessage(message);
            this.message.Timestamp = timestamp;
        }

        public ShortMessageEventArgs(byte status, byte data1, byte data2)
        {
            this.message = new ShortMessage(status, data1, data2);
        }

        public ShortMessage Message
        {
            get
            {
                return message;
            }
        }

        public static ShortMessageEventArgs FromChannelMessage(ChannelMessageEventArgs arg)
        {
            return new ShortMessageEventArgs(arg.Message);
        }

        public static ShortMessageEventArgs FromSysCommonMessage(SysCommonMessageEventArgs arg)
        {
            return new ShortMessageEventArgs(arg.Message);
        }

        public static ShortMessageEventArgs FromSysRealtimeMessage(SysRealtimeMessageEventArgs arg)
        {
            return new ShortMessageEventArgs(arg.Message);
        }
    }
}
