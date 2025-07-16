/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.Structs;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class InstrumentHeldChanged : SeerEvent
    {
        internal InstrumentHeldChanged(EventSource readerBackendType, Instrument instrumentHeld) : base(
            readerBackendType)
        {
            EventType = GetType();
            InstrumentHeld = instrumentHeld;
        }

        public Instrument InstrumentHeld { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}