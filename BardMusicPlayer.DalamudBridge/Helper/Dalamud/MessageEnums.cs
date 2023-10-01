/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.DalamudBridge.Helper.Dalamud;

public enum MessageType
{
    None                    = 0,
    Handshake               = 1,
    Version                 = 2,

    SetGfx                  = 10, //Get<->Set
    NameAndHomeWorld        = 11, //Get
    SetSoundOnOff           = 12, //Set<->Get

    Instrument              = 20,
    NoteOn                  = 21,
    NoteOff                 = 22,
    ProgramChange           = 23,

    StartEnsemble           = 30, //Get<->Set
    AcceptReply             = 31,
    PerformanceModeState    = 32, //Get

    Chat                    = 40,

    NetworkPacket           = 50
}