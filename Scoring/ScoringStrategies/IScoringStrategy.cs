using System.Collections.Generic;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Scoring
{
  public interface IScoringStrategy
  {
    ScoreResult ScoreBin(Collector SubjectBin);
    ScoreResult ScoreList(PresentList BinList, Dictionary<CatchTypes, int> binContents);
    string Slug();
  }
}
