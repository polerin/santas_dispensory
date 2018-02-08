using GameEventBus.Events;

using SMG.Santas.ObjectScripts;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.Coordination.Events
{
  public class ScoreBinEvent : EventBase
  {
    public RoundDefinition Round;
    public Collector Bin;

    public ScoreBinEvent(Collector subjectBin, RoundDefinition subjectRound)
    {
      Round = subjectRound;
      Bin = subjectBin;
    }

  }
}