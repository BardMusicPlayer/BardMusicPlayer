/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal partial class Packet
    {
        /// <summary>
        /// Handles EnsembleStart
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

                var partyLeader = BitConverter.ToUInt32(message, 40);
                if (!ActorIdTools.RangeOkay(partyLeader) || BitConverter.ToUInt32(message, 44) != 0)
                    return;

                _machinaReader.ReaderHandler.Game.PublishEvent(new EnsembleStopped(EventSource.Machina));
            }
            catch (Exception ex)
            {
                _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina,
                    new BmpSeerMachinaException("Exception in Packet.Size88 (ensemble action): " + ex.Message)));
            }
        }
    }
}