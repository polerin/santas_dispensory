using GameEventBus.Events;

using SMG.Santas.GameManagement;

namespace SMG.Santas.Coordination.Events
{
  public class GameEndAfterEvent : EventBase
  {
    public Game Game;

    public GameEndAfterEvent(Game subject)
    {
      Game = subject;
    }
  }
}