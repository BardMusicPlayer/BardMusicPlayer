/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Text;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal sealed partial class Packet
    {
        /// <summary>
        ///     Handles newer game version Player Spawn.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size1168(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;

                if (BitConverter.ToUInt32(message, 0x48) == 0 && 
                    BitConverter.ToUInt32(message, 0x4C) == 0)
                    return;

                var homeWorldId = BitConverter.ToUInt16(message, 114);
                var playerName = Encoding.UTF8.GetString(message, 116, 32).Trim((char)0);
                if (World.Ids.ContainsKey(homeWorldId))
                    _machinaReader.Game.PublishEvent(new HomeWorldChanged(EventSource.Machina,
                        World.Ids[homeWorldId]));

                if (!string.IsNullOrEmpty(playerName))
                    _machinaReader.Game.PublishEvent(new PlayerNameChanged(EventSource.Machina,
                        playerName));
            }
            catch (Exception ex)
            {
                _machinaReader.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina,
                    new BmpSeerMachinaException("Exception in Packet.Size1168 (player spawn): " + ex.Message)));
            }
        }
    }
}