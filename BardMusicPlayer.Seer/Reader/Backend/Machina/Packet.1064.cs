/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal sealed partial class Packet
    {
        /// <summary>
        ///     Handles the performance data-stream
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size1064(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                //if (otherActorId != myActorId)
                //    return;

                var streamData = new List<PerformerStream>();

                for (int i = 37; i != message.Length-3; i+=128)
                {
                    /*var unk1 = message[i];
                    var worldId = BitConverter.ToUInt16(message, i+1); //only set by the first actor
                    var actorId = BitConverter.ToUInt32(message, i+3); //no actor
                    */
                    byte[] notes     = new byte[60];
                    byte[] switches  = new byte[60];
                    Array.Copy(message, i + 8, notes, 0, notes.Length);
                    Array.Copy(message, i + 68, switches, 0, switches.Length);
                    streamData.Add(new PerformerStream
                    {
                        WorldId = BitConverter.ToUInt16(message, i + 1), //only set by the first actor
                        ActorId = BitConverter.ToUInt32(message, i + 3),
                        Notes = notes,
                        Switches = switches
                    });
                }
                if (streamData.Count > 0)
                {
                    if (streamData.Find(n=> n.ActorId == myActorId) != null)
                        _machinaReader.Game.PublishEvent(new EnsembleStreamdata(EventSource.Machina, streamData));
                }

            }
            catch (Exception ex)
            {
                _machinaReader.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina,
                    new BmpSeerMachinaException("Exception in Packet.Size1064: " + ex.Message)));
            }
        }
    }
}