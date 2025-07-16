/*
 * Copyright(c) 2025 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal sealed partial class Packet
    {
        /// <summary>
        ///     Handles EnsembleStart
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size48(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;

                if (BitConverter.ToUInt32(message, 28) != 0 ||
                    BitConverter.ToUInt32(message, 32) != 0 ||
                    BitConverter.ToUInt32(message, 36) != 0 ||
                    BitConverter.ToUInt32(message, 40) != 0 ||
                    BitConverter.ToUInt32(message, 44) != 0)
                    return;

                _machinaReader.Game.PublishEvent(new EnsembleStopped(EventSource.Machina));
            }
            catch (Exception ex)
            {
                _machinaReader.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina,
                    new BmpSeerMachinaException("Exception in Packet.Size88 (ensemble action): " + ex.Message)));
            }
        }
    }
}