using GameEventBus.Interfaces;
using SMG.Santas.GameManagement;
using SMG.Santas.ObjectScripts;

using SMG.Santas.Coordination.Events;

namespace SMG.Santas.RoundFlow
{
  public class BinCountInspector : AbstractRoundInspector
  {

    int maxBins;

    public BinCountInspector(IEventBus<IEvent> EventBus, GameManager GameManager, ScoreboardDisplay Scoreboard) : base(EventBus, GameManager, Scoreboard)
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
      _EventBus.Subscribe<ScoreBinEvent>(HandleBinLimit); ;
      // set the target bins
      maxBins = _GameManager.CurrentRound.maxBins;

      UpdateScoreboard();
    }

    public override void Deactivate()
    {
      _EventBus.Unsubscribe<ScoreBinEvent>(HandleBinLimit);
    }

    public void HandleBinLimit(ScoreBinEvent ScoreEvent)
    {
      if (_GameManager.CurrentRound.binCount >= maxBins) {
        _EventBus.Publish(new RoundSuccessEvent(_GameManager.CurrentRound, GetGameDescription()));
      }
    }
  }
}
