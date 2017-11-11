using System.Collections;
using System.Collections.Generic;
using Scoring;
using UnityEngine;
using UnityEngine.Events;

public class GameManager {
	bool gameOn = false;

	public const string EVENT_GAMESTART = "game_start";

	private UnityAction m_GameStartAction;

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
			this._scoreKeeper = value;
			this._scoreKeeper.OnUpdate += this.UpdateScoreboard;
		}
	}

	protected EventSource _eventSource;
	public EventSource EventSource {
		get { return this._eventSource; }
		set {
			if (this._eventSource != null) {
				this._eventSource.StopListening(GameManager.EVENT_GAMESTART, this.m_GameStartAction);
			}

			this._eventSource = value;
			this._eventSource.StartListening(GameManager.EVENT_GAMESTART, this.m_GameStartAction);
		}
	}

  public delegate void UpdateAction();
  public event UpdateAction OnUpdate;

	/**
	 * Constructor
	 */
	public GameManager (RoundManager RoundManager, ScoreKeeper ScoreKeeper, EventSource EventSource) {
		// Register our StartGame() with our unity action.
		this.m_GameStartAction += this.StartGame;

		this.RoundManager = RoundManager;
		this.ScoreKeeper = ScoreKeeper;
		this.EventSource = EventSource;

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
    // this.ScoreBoard.UpdateText(
    //   bins: 0,
    //   errors: this.RoundManager.Errors(),
    //   score: this.ScoreKeeper.Score,
    //   round: this.RoundManager.Round(),
    //   gameOn: this.gameOn
    // );
  }

  void CheckErrors() {
    if (this.RoundManager.HitMaxErrors()) {
      this.EndGame();
    }
  }

	void StartGame() {
		Debug.Log("STARTING THE DAMN GAAAAME");
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
