using UnityEngine;
using Zenject;

using SMG.Coordination;

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
      if (Input.GetKeyDown(KeyCode.Keypad1)) {
        DispenseBear();
      }

      if (Input.GetKeyDown(KeyCode.Keypad2)) {
        DispenseBall();
      }

      if (Input.GetKeyDown(KeyCode.Keypad3)) {
        DispensePresent();
      }

      if (Input.GetKeyDown(KeyCode.Keypad4)) {
        DispenseHorse();
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
