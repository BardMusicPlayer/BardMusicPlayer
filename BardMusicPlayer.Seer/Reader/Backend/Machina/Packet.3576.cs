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
        /// Contains party information like hp. Does not contain PlayerName. A reference Id is cached for PlayerName lookup in Size928 that comes next.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size3576(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;
                for (var i = 0; i <= 3080; i += 440) // 8 iterations. cache contentId -> actorId. Size928 is fired next which only references the contentId
                {
                    var actorId = BitConverter.ToUInt32(message, 72 + i);
                    if (ActorIdTools.RangeOkay(actorId)) _contentId2ActorId[BitConverter.ToUInt64(message, 64 + i)] = actorId;
                }
            }
            catch (Exception ex)
            {
                _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina, new BmpSeerMachinaException("Exception in Packet.Size3576 (party): " + ex.Message)));
            }
        }
    }
}