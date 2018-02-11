using GameEventBus.Events;

using SMG.Santas.GameManagement;

namespace SMG.Santas.Coordination.Events
{
  public class StartSignalEvent : EventBase
  {
    readonly public GameTypes GameType;

    public StartSignalEvent(GameTypes type)
    {
      GameType = type;
    }
  }
}