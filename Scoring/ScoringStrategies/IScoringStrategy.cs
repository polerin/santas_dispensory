using System.Collections;
using System.Collections.Generic;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Scoring {
  public interface IScoringStrategy {
    PresentList ScoreBin(Collector SubjectBin);
    PresentList ScoreList(PresentList BinList, Dictionary<string, int> binContents);
    string Slug();
  }
}
