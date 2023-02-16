/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using Sanford.Multimedia.Midi;

namespace BardMusicPlayer.Maestro.Old.Utils;

public static class MidiInput
{
    public struct MidiInputDescription
    {
        public string name;
        public int id;
        public MidiInputDescription(string n, int i)
        {
            name = n;
            id = i;
        }
    }

    public static Dictionary<int, string> ReloadMidiInputDevices()
    {
        var midiInputs = new Dictionary<int, string> { { -1, "None" } };
        for (var i = 0; i < InputDevice.DeviceCount; i++)
        {
            var cap = InputDevice.GetDeviceCapabilities(i);
            midiInputs.Add(i, cap.name);
        }
        return midiInputs;
    }
}