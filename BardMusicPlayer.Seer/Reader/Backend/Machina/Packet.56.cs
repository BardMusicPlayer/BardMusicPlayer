/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal partial class Packet
    {
        /// <summary>
        /// Handles Ensemble Request, Ensemble Reject, and Instrument Equip/De-Equip.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size56(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId || BitConverter.ToUInt32(message, 44) != 0) return;

                if (BitConverter.ToUInt16(message, 32) != 2
                    && !(
                        BitConverter.ToUInt32(message, 36) == 16 && BitConverter.ToUInt32(message, 40) > 0 &&
                        BitConverter.ToUInt32(message, 40) < 64 && BitConverter.ToUInt32(message, 44) == 0 &&
                        BitConverter.ToUInt32(message, 48) == 0
                        ||
                        BitConverter.ToUInt32(message, 36) == 1 && BitConverter.ToUInt32(message, 40) == 0 &&
                        BitConverter.ToUInt32(message, 44) == 0 && BitConverter.ToUInt32(message, 48) == 0
                    ))
                {
                    try
                    {
                        if (BitConverter.ToUInt16(message, 48) == 0 && ValidTempo(message[50]) &&
                            ValidTimeSig(message[51])
                        ) // 00 00 [tempo] [timesig]
                        {
                            var partyLeader = BitConverter.ToUInt32(message, 40);
                            if (!ActorIdTools.RangeOkay(partyLeader)) return;
                            _machinaReader.ReaderHandler.Game.PublishEvent(new EnsembleRequested(EventSource.Machina));
                        }
                        else
                        {
                            var partyMember = BitConverter.ToUInt32(message, 40);
                            if (!ActorIdTools.RangeOkay(partyMember)) return;
                            uint reply = message[48];
                            if (reply > 2) return;

                            switch (reply)
                            {
                                case 0: // no instrument equipped reply.
                                    break;
                                case 1: // "ready" reply.
                                    break;
                                case 2: // rejected or timed out replying
                                    _machinaReader.ReaderHandler.Game.PublishEvent(new EnsembleRejected(EventSource.Machina));
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina, new BmpSeerMachinaException("Exception in Packet.Size56 (ensemble action): " + ex.Message)));
                    }
                }
                else
                {
                    try
                    {
                        var category = BitConverter.ToUInt16(message, 32); // category
                        switch (category)
                        {
                            case 2: // instrument equip/dequip is in this category
                                var param1 = BitConverter.ToUInt32(message, 36); // action.
                                var param2 = BitConverter.ToUInt32(message, 40);
                                var param3 = BitConverter.ToUInt32(message, 44);
                                var param4 = BitConverter.ToUInt32(message, 48);
                                if (param3 == 0 && param4 == 0)
                                {
                                    switch (param1)
                                    {
                                        case 16: // equip instrument
                                            _machinaReader.ReaderHandler.Game.PublishEvent(new InstrumentHeldChanged(EventSource.Machina, Instrument.Parse((int) param2)));
                                            break;
                                        case 1: // de-equip instrument
                                            if (param2 == 0) _machinaReader.ReaderHandler.Game.PublishEvent(new InstrumentHeldChanged(EventSource.Machina, Instrument.Parse((int) param2)));
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina, new BmpSeerMachinaException("Exception in Packet.Size56 (equip action): " + ex.Message)));
                    }
                }
            }
            catch (Exception ex)
            {
                _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina, new BmpSeerMachinaException("Exception in Packet.Size56: " + ex.Message)));
            }
        }
    }
}