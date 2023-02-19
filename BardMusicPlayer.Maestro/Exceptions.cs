/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Quotidian;

namespace BardMusicPlayer.Maestro;

public class BmpMaestroException : BmpException
{
    internal BmpMaestroException() { }

    internal BmpMaestroException(string message) : base(message) { }
}

public class BmpMaestroBackendAlreadyRunningException : BmpMaestroException
{
    internal BmpMaestroBackendAlreadyRunningException(EventSource sequencerBackendType) : base("Backend " + sequencerBackendType + " already running")
    {
    }
}
