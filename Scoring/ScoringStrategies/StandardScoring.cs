using System.Collections.Generic;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Scoring
{

  public class StandardScoring : AbstractScoringStrategy
  {
    Dictionary<CatchTypes, int> presentWorths = new Dictionary<CatchTypes, int>()
    {
      {CatchTypes.Ball, 10},
      {CatchTypes.Bear, 3},
      {CatchTypes.Present, 1},
      {CatchTypes.Horse, 5}
    };

    public override PresentList ScoreList(PresentList binList, Dictionary<CatchTypes, int> binContents)
    {
      Dictionary<CatchTypes, int> targetCounts = binList.Counts();
      int listValue = 0;
      int binVal;
      int worthVal;

      foreach (KeyValuePair<CatchTypes, int> type in targetCounts) {
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
