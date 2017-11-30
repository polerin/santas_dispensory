using System.Collections;
using System.Collections.Generic;

namespace SMG.Santas.Scoring {
  public interface IScoringStrategy {
    void ScoreList(PresentList binList, Dictionary<string, int> binContents);
  }
}
