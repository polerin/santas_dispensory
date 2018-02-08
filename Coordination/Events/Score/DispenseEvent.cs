using GameEventBus.Events;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Coordination.Events
{
  public class DispenseEvent : EventBase
  {
    public CatchTypes CatchType;

    public DispenseEvent(CatchTypes type)
    {
      CatchType = type;
    }
  }
}