using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.RoundFlow;
using SMG.Santas.Scoring;
using SMG.Santas.GameManagement;

namespace SMG.Santas.RoundFlow {
  public class BinCountInspector : IRoundInspector {
    UnityAction m_RoundStartAction;
	  UnityAction m_RoundEndAction;

    EventSource _EventSource;
    RoundManager _RoundManager;

    public BinCountInspector (EventSource Source) {
      this._EventSource = Source;

      _EventSource.StopListening(GameManager.EVENT_ROUNDSTART, this.m_RoundStartAction);
    }

    ~BinCountInspector () {
      _EventSource.StopListening(GameManager.EVENT_ROUNDSTART, this.m_RoundStartAction);
    }

    public string Slug () {
      return "binCount";
    }

    public void Inspect(RoundManager Manager) {
      this._RoundManager = Manager;
    }

    public void Activate() {

    }

    public void Deactivate() {
      
    }
  }
}
