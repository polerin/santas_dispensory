using SMG.Coordination;
using SMG.Santas.RoundFlow;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  /// @TODO should the dispenses be individual methods one with a param?
  /// @TODO should this be an event, or should it be a direct call to the Roundmanager?
  public abstract class AbstractControlSet : IControlSet
  {
    protected EventSource _EventSource;
    protected RoundManager _RoundManager;
    protected ScoreboardDisplay _Scoreboard;

    public AbstractControlSet(
      EventSource EventSource,
      RoundManager RoundManager,
      ScoreboardDisplay Scoreboard)
    {
        _EventSource = EventSource;
        _RoundManager = RoundManager;
        _Scoreboard = Scoreboard;
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
      _EventSource.TriggerEvent(Dispense.DISPENSE_PRESENT);
    }

    public abstract string Slug();
    public abstract void Activate();
    public abstract void Deactivate()
  }
}
