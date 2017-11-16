using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager {

  // Notification that a round is about to start
  public const string EVENT_ROUNDSTART = "round_start";

  // Notification that a round is about to end
  public const string EVENT_ROUNDEND = "round_end";

  // Notification that an error is about to be added.
  public const string EVENT_ERRORADDED = "round_error_add";

  // Notification that the player has hit the maximum error count
  public const string EVENT_MAXERRORS = "round_max_errors";

	private UnityAction m_GameStartAction;
  private UnityAction m_GameEndAction;

  RoundDefinition[] Waves;
  ScoreKeeper _ScoreKeeper;
  GameManager _GameManager;
  EventSource _EventSource;

  int maxErrors = 3;

	List<Collector> knownBins = new List<Collector>();
	List<Dispenser> knownDispensers = new List<Dispenser>();

  private int _Round = 0;
  public int Round {
      get { return this._Round; }
      protected set { this._Round = value; }
  }


  private int _Errors = 0;
  public int Errors {
    get { return this._Errors; }
    protected set { this._Errors = value; }
  }


  /**
   * Constructor
   */
  public RoundManager (GameManager Manager, ScoreKeeper Keeper, EventSource Source)
  {
    _GameManager = Manager;
    _ScoreKeeper = Keeper;
    _EventSource = Source;

    // We want to come in after the GameManager has started the game
    _EventSource.StartListening(GameManager.EVENT_GAMESTART_AFTER, m_GameStartAction);
    _EventSource.StartListening(GameManager.EVENT_GAMEEND, m_GameEndAction);

    m_GameStartAction += StartGame;
    m_GameEndAction += EndGame;
  }

  /**
   * Finalizer.  Clean up after ourselves.
   */
  ~RoundManager () {
    if (!_EventSource) {
      return;
    }

    _EventSource.StopListening(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
    _EventSource.StopListening(GameManager.EVENT_GAMEEND, this.m_GameEndAction);
  }

	public void Register(Collector subjectBin)
  {
		if (!knownBins.Contains(subjectBin)) {
			knownBins.Add(subjectBin);
		}
	}

	public void Register(Dispenser subjectDispenser)
  {
		if (!knownDispensers.Contains(subjectDispenser)) {
			knownDispensers.Add(subjectDispenser);
		}
	}

	public void StartGame() {
    Round = 0;
    Errors = 0;
	}

	public void EndGame() {
    EndRound();
	}

	public void StartRound() {
    if (!_GameManager.GameState()) {
      return;
    }

    _EventSource.TriggerEvent(RoundManager.EVENT_ROUNDSTART);
    Round++;
    ResetBins();
    ActivateDispensers();
	}

	public void EndRound() {
    _EventSource.TriggerEvent(RoundManager.EVENT_ROUNDEND);
    DeactivateBins();
	}

  protected void ActivateDispensers() {
    for (int i = 0; i < knownDispensers.Count; i++) {
      knownDispensers[i].Activate();
    }
  }

  protected void ResetBins() {
    for (int i = 0; i < knownBins.Count; i++) {
      knownBins[i].Reset();
    }
  }

  public void DeactivateBins() {
    for (int i = 0; i < knownBins.Count; i++) {
      knownBins[i].Deactivate();
    }
  }

  public void ScoreBin(Collector subjectBin) {
    if (!_ScoreKeeper.ScoreBin(subjectBin)) {
      AddError();
    }

    subjectBin.Reset();
  }

  void AddError() {
    // @TODO figure out how to send back a cancel.  Exception?
    _EventSource.TriggerEvent(RoundManager.EVENT_ERRORADDED);

    Errors += 1;

    if (Errors >= maxErrors) {
      _EventSource.TriggerEvent(RoundManager.EVENT_MAXERRORS);
    }
  }
}
