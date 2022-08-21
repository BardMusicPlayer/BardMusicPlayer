/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Maestro.Events
{
    public sealed class PerformerUpdate : MaestroEvent
    {

        internal PerformerUpdate() : base(0, false)
        {
            EventType = GetType();
        }

        public override bool IsValid() => true;
    }
}
