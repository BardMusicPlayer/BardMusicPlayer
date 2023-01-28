namespace BardMusicPlayer.Jamboree.Events
{
    /// <summary>
    /// if the connection 
    /// </summary>
    public sealed class PartyConnectionChangedEvent : JamboreeEvent
    {
        public enum ResponseCode
        {
            ERROR =-1,
            OK,
            MESSAGE
        }

        internal PartyConnectionChangedEvent(ResponseCode code, string message)
        {
            EventType = GetType();
            Code = code;
            Message = message;
        }

        public ResponseCode Code { get; }

        public string Message { get; }

        public override bool IsValid() => true;
    }
}
