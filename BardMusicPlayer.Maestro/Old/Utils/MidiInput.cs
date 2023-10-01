/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Sanford.Multimedia.Midi;

namespace BardMusicPlayer.Maestro.Old.Utils;

public static class MidiInput
{
    /// <summary>
    /// Helper struct for the Midi-device and Id
    /// </summary>
    public struct MidiInputDescription
    {
        public string name;
        public int id;
        public MidiInputDescription(string n, int i)
        {
            name = n;
            id   = i;
        }
    }

    /// <summary>
    /// Reload and get all midi input devices
    /// </summary>
    /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
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