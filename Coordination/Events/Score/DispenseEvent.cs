using GameEventBus.Events;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Coordination.Events
{
  public class DispenseEvent : EventBase
  {
    readonly public CatchTypes CatchType;

    public DispenseEvent(CatchTypes type)
    {
      CatchType = type;
    }
  }
}