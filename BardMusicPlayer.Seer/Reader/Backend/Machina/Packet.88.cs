/*
 * Copyright(c) 2021 MoogleTroupe
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
        internal void Size88(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;
                var partyLeader = BitConverter.ToUInt32(message, 40);
                if (
                    !ActorIdTools.RangeOkay(partyLeader) ||
                    !(BitConverter.ToUInt16(message, 48) == 0 && ValidTempo(message[50]) && ValidTimeSig(message[51])) || // 00 00 [tempo] [timesig]
                    BitConverter.ToUInt32(message, 44) > 0 || // These should all be zero in an ensemble start packet.
                    BitConverter.ToUInt32(message, 52) > 0 ||
                    BitConverter.ToUInt32(message, 56) > 0 ||
                    BitConverter.ToUInt32(message, 60) > 0 ||
                    BitConverter.ToUInt32(message, 64) > 0 ||
                    BitConverter.ToUInt32(message, 68) > 0 ||
                    BitConverter.ToUInt32(message, 72) > 0 ||
                    BitConverter.ToUInt32(message, 76) > 0
                ) 
                {
                    return;
                }
                _machinaReader.ReaderHandler.Game.PublishEvent(new EnsembleStarted(EventSource.Machina));
            }
            catch (Exception ex)
            {
                _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina, new BmpSeerMachinaException("Exception in Packet.Size88 (ensemble action): " + ex.Message)));
            }
        }
    }
}