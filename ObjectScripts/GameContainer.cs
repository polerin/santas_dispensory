using UnityEngine;
using Zenject;

using SMG.Coordination;
using SMG.Santas.GameManagement;

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

    }
  }
}
