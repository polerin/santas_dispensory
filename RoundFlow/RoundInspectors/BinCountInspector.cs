using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.Scoring;
using SMG.Santas.GameManagement;

namespace SMG.Santas.RoundFlow {
  public class BinCountInspector : AbstractRoundInspector {

    int maxBins;

    public BinCountInspector (EventSource Source, GameManager GameManager)
      :base (Source, GameManager) {

    }

    public override void Activate() {
      _EventSource.StartListening(RoundManager.EVENT_SCOREBIN_AFTER, HandleBinLimit);
      // set the target bins
      maxBins = _GameManager.CurrentRound.maxBins;
    }

    public override void Deactivate() {
      _EventSource.StopListening(RoundManager.EVENT_SCOREBIN_AFTER, HandleBinLimit);
    }

    public void HandleBinLimit() {
      if (_GameManager.CurrentRound.binCount >= maxBins) {
          _EventSource.TriggerEvent(AbstractRoundInspector.EVENT_CONDITION_SUCCESS);
      }
    }
  }
}
