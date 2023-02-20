namespace BardMusicPlayer.DalamudBridge.Helper.Dalamud;

public enum MessageType
{
    None = 0,
    Handshake = 1,
    Version = 2,

    SetGfx = 10,

    Instrument = 20,
    NoteOn = 21,
    NoteOff = 22,
    ProgramChange = 23,

    StartEnsemble = 30,
    AcceptReply = 31,

    Chat = 40
}