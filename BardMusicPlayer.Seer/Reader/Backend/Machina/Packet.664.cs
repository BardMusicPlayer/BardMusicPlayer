/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Text;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal partial class Packet
    {
        /// <summary>
        /// Handles newer game version Player Spawn.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size664(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;
                var homeWorldId = BitConverter.ToUInt16(message, 38);
                var playerName = Encoding.UTF8.GetString(message, 592, 32).Trim((char) 0);

                if (World.Ids.ContainsKey(homeWorldId)) _machinaReader.ReaderHandler.Game.PublishEvent(new HomeWorldChanged(EventSource.Machina, World.Ids[homeWorldId]));
                if (string.IsNullOrEmpty(playerName)) _machinaReader.ReaderHandler.Game.PublishEvent(new PlayerNameChanged(EventSource.Machina, playerName));
            }
            catch (Exception ex)
            {
                _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina, new BmpSeerMachinaException("Exception in Packet.Size664 (player spawn): " + ex.Message)));
            }
        }
    }
}