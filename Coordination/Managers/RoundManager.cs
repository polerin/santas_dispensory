using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager {

  public const string EVENT_ROUNDSTART = "round_start";
  public const string EVENT_ROUNDEND = "round_end";

	private UnityAction m_GameStartAction;
  private UnityAction m_GameEndAction;

  RoundDefinition[] Waves;
  ScoreKeeper _ScoreKeeper;
  GameManager _GameManager;
  EventSource _EventSource;

	List<Collector> knownBins = new List<Collector>();
	List<Dispenser> knownDispensers = new List<Dispenser>();

  int maxErrors = 3;


  // @TODO Remove me once scoreboard is set up with events
  public delegate void UpdateAction();
  public event UpdateAction OnUpdate;

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
    this._GameManager = Manager;
    this._ScoreKeeper = Keeper;

    this._EventSource = Source;
    _EventSource.StartListening(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
    _EventSource.StartListening(GameManager.EVENT_GAMEEND, this.m_GameEndAction);

    // Register our StartGame() with our unity action.
    this.m_GameStartAction += this.StartGame;
    this.m_GameEndAction += this.EndGame;
  }

  /**
   * Finalizer.  Clean up after ourselves.
   */
  ~RoundManager () {
    _EventSource.StopListening(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
    _EventSource.StopListening(GameManager.EVENT_GAMEEND, this.m_GameEndAction);
  }

	public void Register(Collector subjectBin)
  {
		if (!this.knownBins.Contains(subjectBin)) {
			this.knownBins.Add(subjectBin);
		}
	}

	public void Register(Dispenser subjectDispenser)
  {
		if (!this.knownDispensers.Contains(subjectDispenser)) {
			this.knownDispensers.Add(subjectDispenser);
		}
	}

  public bool HitMaxErrors() {
    return (Errors >= maxErrors);
  }

	public void StartGame() {
    this.Round = 0;
    this.Errors = 0;
	}

	public void EndGame() {
    EndRound();
	}

	public void StartRound(int roundNumber) {
    if (!_GameManager.GameState()) {
      return;
    }

    _EventSource.TriggerEvent(RoundManager.EVENT_ROUNDSTART);
    this.ResetBins();
    this.ActivateDispensers();
	}

	public void EndRound() {
    _EventSource.TriggerEvent(RoundManager.EVENT_ROUNDEND);
    this.DeactivateBins();
	}

  protected void ActivateDispensers() {
    for (int i = 0; i < this.knownDispensers.Count; i++) {
      this.knownDispensers[i].Activate();
    }
  }

  protected void ResetBins() {
    for (int i = 0; i < this.knownBins.Count; i++) {
      this.knownBins[i].Reset();
    }
  }

  public void DeactivateBins() {
    for (int i = 0; i < this.knownBins.Count; i++) {
      this.knownBins[i].Deactivate();
    }
  }

  public void ScoreBin(Collector subjectBin) {
    if (!_ScoreKeeper.ScoreBin(subjectBin)) {
      this.AddError();
    }

    subjectBin.Reset();
    this.FireUpdate();
  }

  void AddError() {
    this.Errors += 1;
  }

  // @TODO delete me when the rest of the managers are updated to use events.
  void FireUpdate() {
    if (this.OnUpdate != null) {
      this.OnUpdate();
    }
  }

  bool ShouldClearOnScore() {
    return true;
  }

}
