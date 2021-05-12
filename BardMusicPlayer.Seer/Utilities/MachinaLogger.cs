/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Utilities
{
    internal class MachinaLogger : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }

        public override void WriteLine(string message, string category)
        {
            if ((category?.ToLower().Equals("debug-machina") ?? false) || (category?.ToLower().Equals("firewall") ?? false))
                BmpSeer.Instance.PublishEvent(new MachinaManagerLogEvent(message.Replace(Environment.NewLine, " ")));
        }

        public override bool IsThreadSafe => true;
    }
}
