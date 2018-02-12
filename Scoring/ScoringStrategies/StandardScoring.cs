using System;
using System.Collections.Generic;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Scoring
{

  public class StandardScoring : AbstractScoringStrategy
  {
    Dictionary<CatchTypes, int> PresentWorths = new Dictionary<CatchTypes, int>()
    {
      {CatchTypes.Ball, 10},
      {CatchTypes.Bear, 3},
      {CatchTypes.Present, 1},
      {CatchTypes.Horse, 5}
    };

    /// <summary>
    /// Walk through the target list and the target 
    /// </summary>
    /// <param name="BinList">The target list of desired item counts</param>
    /// <param name="BinContents">The contents of the bin</param>
    /// <returns></returns>
    public override ScoreResult ScoreList(PresentList BinList, Dictionary<CatchTypes, int> BinContents)
    {
      ScoreResult Result = new ScoreResult();
      Dictionary<CatchTypes, int> TargetCounts = BinList.Counts();

      foreach (KeyValuePair<CatchTypes, int> type in TargetCounts) {
        // Dump out early if the bin doesn't have a required key, or the value doesn't match
        if (!BinContents.ContainsKey(type.Key) || BinContents[type.Key] != type.Value) {
          Result.scoreSuccessful = false;
          Result.scoreChange = 0;

          return Result;
        }

        if (!PresentWorths.ContainsKey(type.Key)) {
          throw new InvalidOperationException("StandardScoring does not have a item value for type: " + type.Key);
        }

        Result.scoreChange += type.Value * PresentWorths[type.Key];
      }

      Result.scoreSuccessful = true;
      return Result;
    }

    
  }
}
