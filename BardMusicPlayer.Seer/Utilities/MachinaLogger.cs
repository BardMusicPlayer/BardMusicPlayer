/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Utilities
{
    internal sealed class MachinaLogger : TraceListener
    {
        public override bool IsThreadSafe => true;

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }

        public override void WriteLine(string message, string category)
        {
            if (category?.ToLower().Equals("debug-machina") ?? false)
                BmpSeer.Instance.PublishEvent(new MachinaManagerLogEvent(message.Replace(Environment.NewLine, " ")));
        }
    }
}