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
        internal void Size696(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;

                //get the playername
                byte[] namearray = new byte[32];
                for (int i = 0; i != 32; i++)
                    namearray[i] = (byte)(message[i+ 626] - message[657]);

                var homeWorldId = (int)(BitConverter.ToUInt16(message, 54) - (uint)message[657]);
                var playerName = Encoding.UTF8.GetString(namearray, 0, 32).Trim((char)0);

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
                    new BmpSeerMachinaException("Exception in Packet.Size696 (player spawn): " + ex.Message)));
            }
        }
    }
}