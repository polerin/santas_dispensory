using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using SMG.Coordination;
using SMG.Santas.GameManagement;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.ObjectScripts
{
  public class GameContainer : MonoBehaviour
  {

    private EventSource _EventSource;

    [Inject]
    void Init(EventSource EventSource)
    {
      this._EventSource = EventSource;
    }

    // Update is called once per frame, used here for input management.  Which is crap.
    void Update()
    {
      // @TODO make this not crap

      if (Input.GetKeyDown(KeyCode.N)) {
        this._EventSource.TriggerEvent(GameManager.EVENT_GAMESTART);
      }

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
  }
}
