/*
 * Copyright(c) 2025 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Linq;
using System.Text;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal sealed partial class Packet
    {
        /// <summary>
        ///     Handles Party Invite
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size104(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;
                if (BitConverter.ToUInt32(message, 0x0C) == 0 &&
                    BitConverter.ToUInt32(message, 0x14) == 0 &&
                    BitConverter.ToUInt32(message, 0x1C) == 0)

                    if (message[0x40] == 01)
                    {
                        string inviterName = Encoding.UTF8.GetString(message, 0x42, 32).Trim((char)0);
                        var found = BmpSeer.Instance.Games.Values.FirstOrDefault(n => n.PlayerName == inviterName);
                        if (found == null)
                            return;
                        _machinaReader.Game.PublishEvent(new PartyInvite(EventSource.Machina));
                    }
            }
            catch (Exception ex)
            {
                _machinaReader.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina,
                    new BmpSeerMachinaException("Exception in Packet.Size104 (party invite): " + ex.Message)));
            }
        }
    }
}