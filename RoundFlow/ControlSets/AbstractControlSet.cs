using System;
using UnityEngine;
using GameEventBus.Interfaces;

using SMG.Santas.Coordination.Events;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  /// @TODO should the dispenses be individual methods one with a param?
  public abstract class AbstractControlSet : IControlSet
  {
    protected IEventBus<IEvent> _EventBus;

    protected bool isActive = false;

    public AbstractControlSet(IEventBus<IEvent> EventBus)
    {
      _EventBus = EventBus;

    }

    protected void DispenseItem(CatchTypes DesiredType)
    {
      Debug.Log("Dispensing Item" + DesiredType);
      _EventBus.Publish(new DispenseEvent(DesiredType));
    }

    public abstract string Slug();
    public abstract void Activate();
    public abstract void Deactivate();
  }
}
