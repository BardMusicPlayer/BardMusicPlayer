/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Common;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer
{
    public partial class Game
    {
        private void OnEventReceived(SeerEvent seerEvent)
        {
            try
            {
                // make sure it's valid to begin with and from a backend.
                if (!seerEvent.IsValid() || 10 > (int) seerEvent.EventSource) return;

                seerEvent.Game = this;

                // pass exceptions up immediately
                if (seerEvent is BackendExceptionEvent backendExceptionEvent)
                {
                    Seer.Instance.PublishEvent(backendExceptionEvent);
                    return;
                }

                // deduplicate if needed
                if (seerEvent.DedupeThreshold > 0)
                {
                    if (_eventDedupeHistory.ContainsKey(seerEvent.EventType) && _eventDedupeHistory[seerEvent.EventType] + seerEvent.DedupeThreshold >= seerEvent.TimeStamp) return;
                    _eventDedupeHistory[seerEvent.EventType] = seerEvent.TimeStamp;
                }

                switch (seerEvent)
                {
                    case ActorIdChanged actorId:
                        if (ActorId != actorId.ActorId)
                        {
                            ActorId = actorId.ActorId;
                            Seer.Instance.PublishEvent(actorId);
                        }
                        break;

                    case ChatLog chatLogEvent:
                        // Currently unused.
                        break;

                    case ChatStatusChanged chatStatus:
                        if (ChatStatus != chatStatus.ChatStatus)
                        {
                            ChatStatus = chatStatus.ChatStatus;
                            Seer.Instance.PublishEvent(chatStatus);
                        }
                        break;

                    case ConfigIdChanged configId:
                        if (!ConfigId.Equals(configId.ConfigId))
                        {
                            ConfigId = configId.ConfigId;
                            Seer.Instance.PublishEvent(configId);
                        }
                        break;

                    case EnsembleRejected ensembleRejected:
                        Seer.Instance.PublishEvent(ensembleRejected);
                        break;

                    case EnsembleRequested ensembleRequested:
                        Seer.Instance.PublishEvent(ensembleRequested);
                        break;

                    case EnsembleStarted ensembleStarted:
                        Seer.Instance.PublishEvent(ensembleStarted);
                        break;

                    // Currently unused. Currently unavailable from Machina backend.
                    // case EnsembleStopped ensembleStopped:
                    //    Seer.Instance.PublishEvent(ensembleStopped);
                    //    break;

                    case InstrumentHeldChanged instrumentHeld:
                        if (!InstrumentHeld.Equals(instrumentHeld.InstrumentHeld))
                        {
                            InstrumentHeld = instrumentHeld.InstrumentHeld;
                            Seer.Instance.PublishEvent(instrumentHeld);
                        }
                        break;

                    case KeyMapChanged keyMap:
                        if (!NavigationMenuKeys.Equals(keyMap.NavigationMenuKeys) ||
                            !InstrumentToneMenuKeys.Equals(keyMap.InstrumentToneMenuKeys) ||
                            !InstrumentKeys.Equals(keyMap.InstrumentKeys) ||
                            !InstrumentToneKeys.Equals(keyMap.InstrumentToneKeys) ||
                            !NoteKeys.Equals(keyMap.NoteKeys))
                        {
                            NavigationMenuKeys = keyMap.NavigationMenuKeys;
                            InstrumentToneMenuKeys = keyMap.InstrumentToneMenuKeys;
                            InstrumentKeys = keyMap.InstrumentKeys;
                            InstrumentToneKeys = keyMap.InstrumentToneKeys;
                            NoteKeys = keyMap.NoteKeys;
                            Seer.Instance.PublishEvent(keyMap);
                        }
                        break;

                    case PartyMembersChanged partyMembers:
                        if (!PartyMembers.KeysEquals(partyMembers.PartyMembers))
                        {
                            PartyMembers = partyMembers.PartyMembers;
                            Seer.Instance.PublishEvent(partyMembers);
                        }
                        break;

                    case PlayerNameChanged playerName:
                        if (!PlayerName.Equals(playerName.PlayerName))
                        {
                            PlayerName = playerName.PlayerName;
                            Seer.Instance.PublishEvent(playerName);
                        }
                        break;

                    case HomeWorldChanged homeWorld:
                        if (!HomeWorld.Equals(homeWorld.HomeWorld))
                        {
                            HomeWorld = homeWorld.HomeWorld;
                            Seer.Instance.PublishEvent(homeWorld);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Seer.Instance.PublishEvent(new GameExceptionEvent(this, Pid, ex));
            }
        }
    }
}
