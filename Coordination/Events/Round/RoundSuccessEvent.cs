using GameEventBus.Events;

using SMG.Santas.RoundFlow;
using SMG.Santas.GameManagement;

namespace SMG.Santas.Coordination.Events
{
  public class RoundSuccessEvent : EventBase
  {
    readonly public RoundDefinition Round;
    readonly public GameDescriptor GameDescription;

    public RoundSuccessEvent(RoundDefinition subjectRound, GameDescriptor subjectGameDescription)
    {
      Round = subjectRound;
      GameDescription = subjectGameDescription;
    }    
  }
}