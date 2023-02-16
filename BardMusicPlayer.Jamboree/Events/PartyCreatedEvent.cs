namespace BardMusicPlayer.Jamboree.Events;

/// <summary>
/// Called only on host side
/// </summary>
public sealed class PartyCreatedEvent : JamboreeEvent
{
    /// <summary>
    /// on host, when a party and token was created
    /// </summary>
    /// <param name="token"></param>
    internal PartyCreatedEvent(string token)
    {
        EventType = GetType();
        Token     = token;
    }

    /// <summary>
    /// the base64 token for the clients to join
    /// </summary>
    public string Token { get; }

    public override bool IsValid() => true;
}