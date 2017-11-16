using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoring;

public class ScoreKeeper {

	private UnityAction m_GameStartAction;
	// private UnityAction m_GameEndAction;

	IScoringStrategy ScoringStrategy;
	GameManager _GameManager;

  int currentRound;

	List<int> roundScores = new List<int>();

	private int _Score;
	public int Score {
		get { return _Score; }
		protected set { _Score = value }
	}

	/**
	 * Constructor
	 */
	public ScoreKeeper(GameManager GameManager, IScoringStrategy scoring) {
		this._GameManager = GameManager;
		this._ScoringStrategy = scoring;

		_EventSource.StartListening(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
		// _EventSource.StartListening(GameManager.EVENT_GAMEEND, this.m_GameEndAction);

		// Register our StartGame() with our unity action.
		this.m_GameStartAction += this.StartGame;
		// this.m_GameEndAction += this.EndGame
	}

	~ScoreKeeper() {
		if (!_EventSource) {
			return;
		}

		_EventSource.StopListening(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
		_EventSource.StopListening(GameManager.EVENT_GAMEEND, this.m_GameEndAction)
	}

	public bool ScoreBin(Collector subjectBin) {
		if (!_GameManager.GameState()) {
			return false;
		}

		PresentList binList = subjectBin.GetPresentList();
		Dictionary<string, int> binContents = subjectBin.GetContentCount();

    _ScoringStrategy.ScoreList(binList, binContents);
    AddToScore(binList);

    // add bin checking logic
    return binList.SuccessfulScoring();
	}

	void AddToScore(PresentList binList) {
    AddToScore(binList.Score());
  }

  void AddToScore(int points) {
    currentScore += points;
    roundScores[currentRound] += points;
  }

	public void StartGame() {
    currentScore = 0;
	}

  public void StartRound(int roundNumber) {
		if (!_GameManager.GameState()) {
			return;
		}

		if (roundScores.Count > roundNumber) {
			roundScores[roundNumber] = 0;
		} else {
			roundScores.Insert(roundNumber, 0);
		}

		currentRound = roundNumber;
	}
}
