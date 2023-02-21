/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayerApi/blob/develop/LICENSE for full license information.
 */

using System.Threading;
using BardMusicPlayer.Maestro.Sequencer;

namespace BardMusicPlayer.Maestro.ApiTest;

internal class Program
{
    static void Main()
    {
        BmpMaestro.Instance.Start(SequencerType.MogAmp);

        Thread.Sleep(5000);

        BmpMaestro.Instance.Stop();
    }
}