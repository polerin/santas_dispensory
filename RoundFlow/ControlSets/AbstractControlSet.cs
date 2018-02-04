using SMG.Coordination;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  /// @TODO should the dispenses be individual methods one with a param?
  public abstract class AbstractControlSet : IControlSet
  {
    protected EventSource _EventSource;

    protected bool isActive = false;

    public AbstractControlSet(EventSource EventSource)
    {
        _EventSource = EventSource;
    }

    protected void DispenseBear()
    {
      _EventSource.TriggerEvent(Dispenser.DISPENSE_BEAR);
    }

    protected void DispenseBall()
    {
      _EventSource.TriggerEvent(Dispenser.DISPENSE_BALL);
    }

    protected void DispensePresent()
    {
      _EventSource.TriggerEvent(Dispenser.DISPENSE_PRESENT);
    }

    protected void DispenseHorse()
    {
      _EventSource.TriggerEvent(Dispenser.DISPENSE_PRESENT);
    }

    public abstract string Slug();
    public abstract void Activate();
    public abstract void Deactivate();
  }
}
