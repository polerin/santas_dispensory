using GameEventBus.Events;

using SMG.Santas.GameManagement;

namespace SMG.Santas.Coordination.Events
{
  public class StartSignalEvent : EventBase
  {
    public GameTypes GameType;

    public StartSignalEvent(GameTypes type)
    {
      GameType = type;
    }
  }
}