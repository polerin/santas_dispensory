using GameEventBus.Events;
using SMG.Santas.RoundFlow;
using SMG.Santas.GameManagement;

namespace SMG.Santas.Coordination.Events
{
  public class RoundFailedEvent : EventBase
  {
    public RoundDefinition Round;
    public Game Game;

    public RoundFailedEvent(RoundDefinition subject, Game subjectGame)
    {
      Round = subject;
      Game = subjectGame;
    }

  }
}