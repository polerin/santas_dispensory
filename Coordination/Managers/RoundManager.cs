using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager {
  RoundDefinition[] Waves;
  ScoreKeeper ScoreKeeper;

	List<Collector> knownBins = new List<Collector>();
	List<Dispenser> knownDispensers = new List<Dispenser>();

  int currentRound = 0;
  int errorCount = 0;
  int maxErrors = 3;
  bool gameOn = false;

  public delegate void UpdateAction();
  public event UpdateAction OnUpdate;

  public delegate void DispenseAction();
  public event DispenseAction OnDispense;

  public int Round() {
    return this.currentRound;
  }

  public int Errors() {
    return this.errorCount;
  }

  public bool HitMaxErrors() {
    return (this.errorCount >= this.maxErrors);
  }

	public void Register(Collector subjectBin) {
		if (!this.knownBins.Contains(subjectBin)) {
      this.InitBin(subjectBin);
			this.knownBins.Add(subjectBin);
		}
	}

	public void Register(Dispenser subjectDispenser) {
		if (!this.knownDispensers.Contains(subjectDispenser)) {
      this.OnDispense += subjectDispenser.DispenseItem;
			this.knownDispensers.Add(subjectDispenser);
		}
	}

  public void AddScoreKeeper(ScoreKeeper keeper) {
    this.ScoreKeeper = keeper;
  }

	public void InitBin(Collector subjectBin) {
		if (!subjectBin.State(subjectBin.StateReady)) {
			PresentList newList = new PresentList();
			subjectBin.AddPresentList(newList);
			subjectBin.Wait();
		}

	}


	public void StartGame() {
    this.gameOn = true;
    this.currentRound = 0;
    this.errorCount = 0;
	}

	public void EndGame() {
    this.gameOn = false;
	}

	public void StartRound(int roundNumber) {
    if (!this.gameOn) {
      Debug.Log("Not starting round");
      return;
    }

    this.ResetBins();
    this.ActivateDispensers();
    Debug.Log("starting round");
	}

  protected void ActivateDispensers() {
    for (int i = 0; i < this.knownDispensers.Count; i++) {
      this.knownDispensers[i].Activate();
    }
  }

  protected void ResetBins() {
    for (int i = 0; i < this.knownBins.Count; i++) {
      this.ResetBin(this.knownBins[i]);
    }
  }

  // @todo this might get moved off into a Stage strategy
	public void ResetBin(Collector subjectBin) {
    subjectBin.Wait(false);
		subjectBin.GetPresentList().GenerateRandomCounts();
    subjectBin.EmptyCollector();
	}

  public void DeactivateBins() {
    for (int i = 0; i < this.knownBins.Count; i++) {
      this.knownBins[i].Wait(true);
    }
  }

  public void ScoreBin(Collector subjectBin) {
    if (!this.ScoreKeeper.ScoreBin(subjectBin)) {
      this.AddError();
    }
    this.ResetBin(subjectBin);

    this.FireUpdate();
  }

	public void EndRound() {

	}

  void AddError() {
    this.errorCount += 1;
  }

  void FireUpdate() {
    if (this.OnUpdate != null) {
      this.OnUpdate();
    }
  }


  bool ShouldClearOnScore() {
    return true;
  }

	// Use this for initialization
	void Start () {

	}
}
