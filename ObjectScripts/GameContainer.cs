using UnityEngine;
using Zenject;
using GameEventBus.Interfaces;

using SMG.Santas.GameManagement;
using SMG.Santas.Coordination.Events;

namespace SMG.Santas.ObjectScripts
{
  public class GameContainer : MonoBehaviour
  {

    private IEventBus<IEvent> _EventBus;

    [Inject]
    void Init(IEventBus<IEvent> EventBus)
    {
      _EventBus = EventBus;
    }

    // Update is called once per frame, used here for input management.  Which is crap.
    void Update()
    {
      // @TODO make this not crap

      if (Input.GetKeyDown(KeyCode.S)) {
        _EventBus.Publish(new StartSignalEvent(GameTypes.SoloNormal));
      }


      if (Input.GetKeyDown(KeyCode.P)) {
        _EventBus.Publish(new StartSignalEvent(GameTypes.PartnerNormal));
      }

    }
  }
}
