using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SMG.Santas.Scoring;

namespace SMG.Santas.Scoring {

  public class StandardScoring : AbstractScoringStrategy {
    Dictionary<string, int> presentWorths = new Dictionary<string,int>()
    {
      {"present", 1},
      {"horse", 2},
      {"bear", 5},
      {"ornament", 10}
    };

    public override PresentList ScoreList(PresentList binList, Dictionary<string, int> binContents) {
      Dictionary<string, int> targetCounts = binList.Counts();
      int listValue = 0;
      int binVal;
      int worthVal;

      foreach (KeyValuePair<string, int> type in targetCounts) {
        binContents.TryGetValue(type.Key, out binVal);
        if (type.Value == binVal) {
          this.presentWorths.TryGetValue(type.Key, out worthVal);
          listValue += type.Value * worthVal;

        } else {
          binList.SuccessfulScoring(false);
          // Early exit when list scoring is not successful.
          return binList;
        }
      }

      binList.Score(listValue);
      binList.SuccessfulScoring(true);

      return binList;
    }
  }
}
