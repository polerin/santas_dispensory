using GameEventBus.Events;

using SMG.Santas.ObjectScripts;
using SMG.Santas.RoundFlow;
using SMG.Santas.Scoring;

namespace SMG.Santas.Coordination.Events
{
  public class ScoreBinEvent : EventBase
  {
    readonly public RoundDefinition Round;
    readonly public Collector Bin;
    readonly public ScoreResult ScoreResult;

    public ScoreBinEvent(ScoreResult subjectScore, Collector subjectBin, RoundDefinition subjectRound)
    {
      Round = subjectRound;
      Bin = subjectBin;
      ScoreResult = subjectScore;
    }

  }
}