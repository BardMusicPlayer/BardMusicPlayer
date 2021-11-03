using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Maestro
{
    public sealed class CurrentPlayPositionEvent : MaestroEvent
    {
        internal CurrentPlayPositionEvent(ITimeSpan inTimeSpan) : base(0, false)
        {
            EventType = GetType();
            timeSpan = inTimeSpan;
        }

        public ITimeSpan timeSpan { get; }

        public override bool IsValid() => true;
    }

}
