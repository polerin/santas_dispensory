using GameEventBus.Events;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.Coordination.Events
{
  public class RoundEndEvent : EventBase
  {
    readonly public RoundDefinition Round;

    public RoundEndEvent(RoundDefinition subject)
    {
      Round = subject;
    }
    
  }
}