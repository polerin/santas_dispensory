using System.Collections;
using System.Collections.Generic;
using Scoring;
using UnityEngine;
using UnityEngine.Events;

public class GameManager {
	bool gameOn = false;

	// Issued to start the game.  (not issued by game manager)
	public const string EVENT_GAMESTART = "game_start";

	// Issued to signal that the game has just started
	public const string EVENT_GAMESTART_AFTER = "game_start_after";

  // Issued to signal that the game is ending
	public const string EVENT_GAMEEND = "game_end";

	//Issued to signal that the game has just ended
	public const string EVENT_GAMEEND_AFTER = "game_end_after";

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


	// @TODO I don't think the game manager needs the score keeper any more, double check.
  protected ScoreKeeper _scoreKeeper;
  public ScoreKeeper ScoreKeeper {
		get { return this._scoreKeeper; }
		private set {
			this._scoreKeeper = value;
		}
	}

	protected EventSource _EventSource;
	public EventSource EventSource {
		get { return this._EventSource; }
		set {
			this._EventSource = value;
			this._EventSource.StartListening(GameManager.EVENT_GAMESTART, this.m_GameStartAction);
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

  ~GameManager() {
		_EventSource.StopListening(GameManager.EVENT_GAMESTART, this.m_GameStartAction);
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
		this.gameOn = true;
		_EventSource.TriggerEvent(GameManager.EVENT_GAMESTART_AFTER);

		// @TODO remove me once the Round manager and scoreboard are updated for events
    this.StartRound();
	}

  void EndGame() {
		_EventSource.TriggerEvent(GameManager.EVENT_GAMEEND);
    this.gameOn = false;
		_EventSource.TriggerEvent(GameManager.EVENT_GAMEEND_AFTER);

    this.EndRound();
    this.RoundManager.EndGame();
    this.ScoreKeeper.EndGame();

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
