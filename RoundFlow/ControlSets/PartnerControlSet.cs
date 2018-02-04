using UnityEngine;
using Zenject;

using SMG.Coordination;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  public class PartnerControlSet : AbstractControlSet, ITickable
  {
    /// <summary>
    /// Injected settings, set in the Installer via the Unity Inspector.
    /// </summary>
    Settings _Settings;

    /// <summary>
    /// Injected Zenject TickableManager.  Used in Activate() and Deactivate()
    /// so the Tick() method isn't called when this set is not active.
    /// </summary>
    TickableManager _TickableManager;

    public PartnerControlSet(
      EventSource EventSource,
      TickableManager TickableManager,
      Settings Settings)
      : base(EventSource)
    {
      _Settings = Settings;
      _TickableManager = TickableManager;
    }

    public void Tick()
    {
      // @TODO make this not crap
      Debug.Log("TICK TICK TICK");
      if (Input.GetKeyDown(KeyCode.Keypad1)) {
        this._EventSource.TriggerEvent(Dispenser.DISPENSE_BEAR);
      }


      if (Input.GetKeyDown(KeyCode.Keypad2)) {
        this._EventSource.TriggerEvent(Dispenser.DISPENSE_BALL);
      }

      if (Input.GetKeyDown(KeyCode.Keypad3)) {
        this._EventSource.TriggerEvent(Dispenser.DISPENSE_PRESENT);
      }

      if (Input.GetKeyDown(KeyCode.Keypad4)) {
        this._EventSource.TriggerEvent(Dispenser.DISPENSE_HORSE);
      }
    }
    public override string Slug()
    {
      return "partnerControlSet";
    }

    public override void Activate()
    {
      Debug.Log("Activating: " + Slug());
      isActive = true;
      _TickableManager.Add(this);
    }

    public override void Deactivate()
    {
      Debug.Log("Deactivating: " + Slug());
      isActive = false;
      _TickableManager.Remove(this);
    }
    public class Settings
    {

    }
  }
}
