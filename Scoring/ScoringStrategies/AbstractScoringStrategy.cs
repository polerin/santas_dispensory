using System.Collections.Generic;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Scoring
{
  public abstract class AbstractScoringStrategy : IScoringStrategy
  {

    public string Slug()
    {
      return this.GetType().Name;
    }

    public ScoreResult ScoreBin(Collector SubjectBin)
    {
      return ScoreList(SubjectBin.GetPresentList(), SubjectBin.GetContentCount());
    }

    public abstract ScoreResult ScoreList(PresentList binList, Dictionary<CatchTypes, int> binContents);
  }
}
