using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoring;

public class GameManager {
  // @todo these values will probably be moved into a scoring strategy
	bool gameOn = false;

	protected ScoreboardDisplay ScoreBoard;

	protected RoundManager _roundManager;
  public RoundManager RoundManager {
		get { return this._roundManager; }
		private set {
			this._roundManager = value;
			this._roundManager.OnUpdate += this.UpdateScoreboard;
			this._roundManager.OnUpdate += this.CheckErrors;
		}
	}

  protected ScoreKeeper _scoreKeeper;
  public ScoreKeeper ScoreKeeper {
		get { return this._scoreKeeper; }
		private set {
			this.ScoreKeeper = value;
			this.ScoreKeeper.OnUpdate += this.UpdateScoreboard;
		}
	}


  public delegate void UpdateAction();
  public event UpdateAction OnUpdate;

	/**
	 * Constructor
	 */
	public GameManager (RoundManager RoundManager, ScoreKeeper ScoreKeeper) {
		this.RoundManager = RoundManager;
		this.ScoreKeeper = ScoreKeeper;

		// Register to our own update event
		this.OnUpdate += this.UpdateScoreboard;
	}

  public bool GameState() {
    return gameOn;
  }

  void FireUpdate() {
    if (OnUpdate != null) {
      OnUpdate();
    }
  }

  void UpdateScoreboard() {
    this.ScoreBoard.UpdateText(
      bins: 0,
      errors: this.RoundManager.Errors(),
      score: this.ScoreKeeper.Score,
      round: this.RoundManager.Round(),
      gameOn: this.gameOn
    );
  }

  void CheckErrors() {
    if (this.RoundManager.HitMaxErrors()) {
      this.EndGame();
    }
  }

	void StartGame() {
    this.RoundManager.StartGame();
    this.ScoreKeeper.StartGame();

		this.gameOn = true;
    this.StartRound();
	}

  void EndGame() {
    this.EndRound();
    this.RoundManager.EndGame();
    this.ScoreKeeper.EndGame();

    this.gameOn = false;
	}

  void StartRound(int roundNumber = 0) {
    this.RoundManager.StartRound(roundNumber);
    this.ScoreKeeper.StartRound(roundNumber);

    this.FireUpdate();
	}

	void EndRound() {
    this.RoundManager.EndRound();
    this.ScoreKeeper.EndRound();
	}

}
