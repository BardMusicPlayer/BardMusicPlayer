/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro.Utils
{
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
            Dictionary<int, string> midiInputs = new Dictionary<int, string>();
            midiInputs.Add(-1, "None");
            for (int i = 0; i < InputDevice.DeviceCount; i++)
            {
                MidiInCaps cap = InputDevice.GetDeviceCapabilities(i);
                midiInputs.Add(i, cap.name);
            }
            return midiInputs;
        }
    }
}
