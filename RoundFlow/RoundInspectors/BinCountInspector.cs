using SMG.Coordination;
using SMG.Santas.GameManagement;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  public class BinCountInspector : AbstractRoundInspector
  {

    int maxBins;

    public BinCountInspector(EventSource Source, GameManager GameManager, ScoreboardDisplay Scoreboard) : base(Source, GameManager, Scoreboard)
    {
    }

    ~BinCountInspector()
    {
      Deactivate();
    }

    public override string Slug()
    {
      return "binCount";
    }

    protected override string GetDetailLabel()
    {
      return "Required Bins";
    }

    protected override string GetDetailValue()
    {
      return _GameManager.CurrentRound.binCount.ToString() + " / " + maxBins.ToString();
    }

    public override void Activate()
    {
      _EventSource.StartListening(RoundManager.EVENT_SCOREBIN_AFTER, HandleBinLimit);
      // set the target bins
      maxBins = _GameManager.CurrentRound.maxBins;

      UpdateScoreboard();
    }

    public override void Deactivate()
    {
      _EventSource.StopListening(RoundManager.EVENT_SCOREBIN_AFTER, HandleBinLimit);
    }

    public void HandleBinLimit()
    {
      if (_GameManager.CurrentRound.binCount >= maxBins) {
        _EventSource.TriggerEvent(AbstractRoundInspector.EVENT_CONDITION_SUCCESS);
      }
    }
  }
}
