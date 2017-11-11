using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoring;

public class ScoreKeeper {
	bool gameOn = false;
  int currentRound;

	int currentScore = 0;
	public int Score {
		get { return currentScore; }
	}

	List<int> roundScores = new List<int>();

	IScoringStrategy ScoringStrategy;

	public delegate void UpdateAction();
  public event UpdateAction OnUpdate;

	/**
	 * Constructor
	 */
	public ScoreKeeper(IScoringStrategy scoring) {
		this.ScoringStrategy = scoring;
	}

	public bool ScoreBin(Collector subjectBin) {
		if (!this.gameOn) {
			return false;
		}

		PresentList binList = subjectBin.GetPresentList();
		Dictionary<string, int> binContents = subjectBin.GetContentCount();

    this.ScoringStrategy.ScoreList(binList, binContents);
    this.AddToScore(binList);

    // add bin checking logic
    return binList.SuccessfulScoring();
	}

	void AddToScore(PresentList binList) {
    this.AddToScore(binList.Score());
  }

  void AddToScore(int points) {
    this.currentScore += points;
    this.roundScores[this.currentRound] += points;
  }

	public void StartGame() {
    this.currentScore = 0;
		this.gameOn = true;
	}

  public void EndGame() {
		this.gameOn = false;
	}

  public void StartRound(int roundNumber) {
		if (!this.gameOn) {
			return;
		}

		if (this.roundScores.Count > roundNumber) {
			this.roundScores[roundNumber] = 0;
		} else {
			this.roundScores.Insert(roundNumber, 0);
		}

		this.currentRound = roundNumber;
	}

	public void EndRound() {

	}

  void FireUpdate() {
    if (this.OnUpdate != null) {
      this.OnUpdate();
    }
  }


}
