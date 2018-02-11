using GameEventBus.Events;

using SMG.Santas.GameManagement;

namespace SMG.Santas.Coordination.Events
{
  public class GameEndEvent : EventBase
  {
    readonly public Game Game;

    public GameEndEvent(Game subject)
    {
      Game = subject;
    }
  }
}