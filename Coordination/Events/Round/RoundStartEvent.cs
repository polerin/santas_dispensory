using GameEventBus.Events;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.Coordination.Events
{
  public class RoundStartEvent : EventBase
  {
    public RoundDefinition Round;

    public RoundStartEvent(RoundDefinition subject)
    {
      Round = subject;
    }
    
  }
}