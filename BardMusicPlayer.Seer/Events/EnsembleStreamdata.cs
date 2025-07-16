/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Collections.Generic;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class PerformerStream
    {
        public ushort WorldId { get; set; }
        public uint ActorId { get; set; }
        public byte[] Notes = new byte[60];
        public byte[] Switches = new byte[60];
    }

    public sealed class EnsembleStreamdata : SeerEvent
    {
        internal EnsembleStreamdata(EventSource readerBackendType, List<PerformerStream> streamdata) : base(readerBackendType, 100,
            true)
        {
            EventType = GetType();
            StreamData = streamdata;
        }

        public List<PerformerStream> StreamData { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}