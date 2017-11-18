using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentList {
  Dictionary<string, int> presentCounts = new Dictionary<string, int>();

  int maxCount = 3;
  int minCount = 1;

  int lastScoreValue = 0;
  bool lastScoreSuccessful = false;


  public PresentList() {
    // init our counts
    this.presentCounts.Add("horse", 0);
    this.presentCounts.Add("bear", 0);
    this.presentCounts.Add("present", 0);
    this.presentCounts.Add("ornament", 0);

    // mark when the list was created
    this.MarkStart();
  }

  public void GenerateRandomCounts() {
    Dictionary<string, int> temp = new Dictionary<string, int>();

    foreach (string key in presentCounts.Keys) {
      temp[key] = Random.Range(this.minCount, this.maxCount);
    }

    foreach (KeyValuePair<string, int> newVals in temp) {
      this.SetTypeCount(newVals.Key, newVals.Value);
    }
  }

  public void SetTypeCount(string type, int count) {
    if (!this.presentCounts.ContainsKey(type)) {
      return;
    }

    if (count < this.minCount) {
      count = this.minCount;
    } else if (count > maxCount){
      count = this.maxCount;
    }

    this.presentCounts[type] = count;
  }

  public Dictionary<string, int> Counts() {
    return new Dictionary<string, int>(presentCounts);
  }

  public void Duration() {

  }


  public bool SuccessfulScoring() {
    return this.lastScoreSuccessful;
  }

  public void SuccessfulScoring(bool success) {
    this.lastScoreSuccessful = success;
  }

  public int Score() {
    return this.lastScoreValue;
  }

  public void Score(int incomingScore) {
    this.lastScoreValue = incomingScore;
  }

  public void SetMaxCount(int newMax) {
    if (newMax <= this.minCount) {
      newMax = this.minCount + 1;
    }

    this.maxCount = newMax;
  }

  public void SetMinCount(int newMin) {
    if (newMin < 0) {
      newMin = 0;
    }

    if (newMin >= this.maxCount) {
      newMin = this.maxCount - 1;
    }

    this.minCount = 0;
  }

  void MarkStart() {}
  void MarkEnd() {}
}
