using GameEventBus.Events;

using SMG.Santas.GameManagement;

namespace SMG.Santas.Coordination.Events
{
  public class GameStartEvent : EventBase
  {
    readonly public Game Game;

    public GameStartEvent(Game subject)
    {
      Game = subject;
    }
  }
}