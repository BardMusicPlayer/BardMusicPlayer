/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Events
{
    internal sealed class ExceptionEvent : EventArgs
    {
        public ExceptionEvent(object sender, Exception exception)
        {
            Sender = sender;
            Exception = exception;
        }

        public Exception Exception { get; set; }

        public object Sender { get; set; }
    }
}