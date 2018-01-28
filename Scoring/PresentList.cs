using System.Collections.Generic;
using UnityEngine;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Scoring
{
  public class PresentList
  {
    Dictionary<CatchTypes, int> presentCounts = new Dictionary<CatchTypes, int>();

    private int maxCount = 3;
    private int minCount = 1;

    private int lastScoreValue = 0;
    private bool lastScoreSuccessful = false;

    public PresentList()
    {
      // init our counts
      this.presentCounts.Add(CatchTypes.Bear, 0);
      this.presentCounts.Add(CatchTypes.Ball, 0);
      this.presentCounts.Add(CatchTypes.Present, 0);
      this.presentCounts.Add(CatchTypes.Horse, 0);

      // mark when the list was created
      this.MarkStart();
    }

    public void GenerateRandomCounts()
    {
      Dictionary<CatchTypes, int> temp = new Dictionary<CatchTypes, int>();

      foreach (CatchTypes key in presentCounts.Keys) {
        temp[key] = Random.Range(this.minCount, this.maxCount);
      }

      foreach (KeyValuePair<CatchTypes, int> newVals in temp) {
        this.SetTypeCount(newVals.Key, newVals.Value);
      }
    }

    public void SetTypeCount(CatchTypes type, int count)
    {
      if (!this.presentCounts.ContainsKey(type)) {
        return;
      }

      if (count < this.minCount) {
        count = this.minCount;
      } else if (count > maxCount) {
        count = this.maxCount;
      }

      this.presentCounts[type] = count;
    }

    public Dictionary<CatchTypes, int> Counts()
    {
      return new Dictionary<CatchTypes, int>(presentCounts);
    }

    public void Duration()
    {

    }


    public bool SuccessfulScoring()
    {
      return this.lastScoreSuccessful;
    }

    public void SuccessfulScoring(bool success)
    {
      this.lastScoreSuccessful = success;
    }

    public int Score()
    {
      return this.lastScoreValue;
    }

    public void Score(int incomingScore)
    {
      this.lastScoreValue = incomingScore;
    }

    public void SetMaxCount(int newMax)
    {
      if (newMax <= this.minCount) {
        newMax = this.minCount + 1;
      }

      this.maxCount = newMax;
    }

    public void SetMinCount(int newMin)
    {
      if (newMin < 0) {
        newMin = 0;
      }

      if (newMin >= this.maxCount) {
        newMin = this.maxCount - 1;
      }

      this.minCount = 0;
    }

    void MarkStart() { }
    void MarkEnd() { }
  }
}
