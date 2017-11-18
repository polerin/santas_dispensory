using System.Collections;
using System.Collections.Generic;

namespace Scoring {
  public interface IScoringStrategy {
    void ScoreList(PresentList binList, Dictionary<string, int> binContents);
  }
}
