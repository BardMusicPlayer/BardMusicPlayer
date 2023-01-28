namespace BardMusicPlayer.Jamboree.Events
{
    /// <summary>
    /// if the connection 
    /// </summary>
    public sealed class PartyChangedEvent : JamboreeEvent
    {
        internal PartyChangedEvent()
        {
            EventType = GetType();
        }

        public override bool IsValid() => true;
    }
}
