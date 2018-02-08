using UnityEngine;
using Zenject;
using GameEventBus.Interfaces;

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
      IEventBus<IEvent> EventBus,
      TickableManager TickableManager,
      Settings Settings)
      : base(EventBus)
    {
      _Settings = Settings;
      _TickableManager = TickableManager;
    }

    public void Tick()
    {
      // @TODO make this not annoying
      if (Input.GetKeyDown(KeyCode.Keypad1)) {
        DispenseItem(CatchTypes.Bear);
      }

      if (Input.GetKeyDown(KeyCode.Keypad2)) {
        DispenseItem(CatchTypes.Ball);
      }

      if (Input.GetKeyDown(KeyCode.Keypad3)) {
        DispenseItem(CatchTypes.Present);
      }

      if (Input.GetKeyDown(KeyCode.Keypad4)) {
        DispenseItem(CatchTypes.Horse);
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
