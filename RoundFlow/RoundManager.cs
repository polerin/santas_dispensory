using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.Scoring;
using SMG.Santas.RoundFlow;
using SMG.Santas.GameManagement;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow {
	public class RoundManager {
		public const string EVENT_SCOREBIN = "round_scorebin";
		public const string EVENT_SCOREBIN_AFTER = "round_scorebin_after";

		UnityAction m_GameStartAction;
		UnityAction m_RoundStartAction;
	  UnityAction m_RoundEndAction;

	  public RoundDefinition CurrentRound {
			get { return _GameManager.CurrentRound; }
			protected set {}
		}

	  IRoundInspector CurrentRoundInspector;
		IScoringStrategy CurrentScoringStrategy;
	  ScoreKeeper _ScoreKeeper;
	  GameManager _GameManager;
	  EventSource _EventSource;

	  int maxErrors = 3;
	  string defaultRoundType = "binCount";
	  string defaultScoringType = "StandardScoring";

	  Dictionary<string, IRoundInspector> _RoundInspectors = new Dictionary<string, IRoundInspector>();
	  Dictionary<string, IScoringStrategy> _ScoringStrategies = new Dictionary<string, IScoringStrategy>();
		List<Collector> knownBins = new List<Collector>();
		List<Dispenser> knownDispensers = new List<Dispenser>();

	  /**
	   * Constructor
	   */
	  public RoundManager (GameManager Manager, ScoreKeeper Keeper, EventSource Source,
			List<IRoundInspector> RoundInspectors, List<IScoringStrategy> ScoringStrategies)
	  {
	    _GameManager = Manager;
	    _ScoreKeeper = Keeper;
	    _EventSource = Source;

	    // We want to come in after the GameManager has started the game
	    // _EventSource.StartListening(GameManager.EVENT_GAMESTART_AFTER, m_GameStartAction);
	    _EventSource.StartListening(GameManager.EVENT_GAMEEND, m_RoundEndAction);
	    _EventSource.StartListening(GameManager.EVENT_ROUNDEND, m_RoundEndAction);

	    m_GameStartAction += StartRound;
	    m_RoundEndAction += EndRound;

	    for (int i = 0; i < RoundInspectors.Count; i++) {
	      RoundInspectors[i].Inspect(this);
	      _RoundInspectors.Add(RoundInspectors[i].Slug(), RoundInspectors[i]);
	    }

			for (int i = 0; i < ScoringStrategies.Count; i++) {
				_ScoringStrategies.Add(ScoringStrategies[i].Slug(), ScoringStrategies[i]);
			}
	  }

	  /**
	   * Finalizer.  Clean up after ourselves.
	   */
	  ~RoundManager () {
			Debug.Log("Roundmanager destructor");
	    if (_EventSource == null) {
	      return;
	    }

	    // _EventSource.StopListening(GameManager.EVENT_GAMESTART_AFTER, StartRound);
	    _EventSource.StopListening(GameManager.EVENT_GAMEEND, m_RoundEndAction);
	    _EventSource.StopListening(GameManager.EVENT_ROUNDEND, m_RoundEndAction);
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

	  /**
	   * Load up the right round inspector, activate the appropriate bins and dispensers.
	   * @TODO when we have event params figured out, use the passed RoundDefinition
	   * @type {[type]}
	   */
		protected void StartRound() {
			Debug.Log("RM StartRound");
	    if (!_GameManager.GameState()) {
	      return;
	    }

	    RoundDefinition CurrentRound = _GameManager.CurrentRound;

	    SetRoundManager(CurrentRound);
			SetScoringStrategy(CurrentRound);
	    ResetBins(CurrentRound);
	    ActivateDispensers(CurrentRound);
		}

		protected void EndRound() {
	    if (!_GameManager.GameState()) {
	      return;
	    }

	    DeactivateBins();
	  }

		// @TODO exception for default round manager not found
	  protected void SetRoundManager(RoundDefinition Round) {
	    IRoundInspector TargetInspector = null;

	    if (_RoundInspectors.TryGetValue(Round.roundType, out TargetInspector)) {
	      CurrentRoundInspector = TargetInspector;
	    } else {
	      Debug.Log("Round type not found in inspector list: " + Round.roundType);
	      CurrentRoundInspector = _RoundInspectors[defaultRoundType];
	    }

	    CurrentRoundInspector.Activate();
	  }

		protected void SetScoringStrategy(RoundDefinition Round) {
			IScoringStrategy TargetStrategy = null;

			if (_ScoringStrategies.TryGetValue(Round.scoreType, out TargetStrategy)) {
				CurrentScoringStrategy = TargetStrategy;
			} else {
				Debug.Log("Scoring type not found in strategy list: " + Round.scoreType);
				CurrentScoringStrategy = _ScoringStrategies[defaultScoringType];
			}

			CurrentRoundInspector.Activate();
		}

	  /**
	   * Activate the appropriate dispensers based on round definition.
	   * @TODO Use the round definition.
	   * @type {Number}
	   */
	  protected void ActivateDispensers(RoundDefinition Round) {
	    for (int i = 0; i < knownDispensers.Count; i++) {
	      knownDispensers[i].Activate();
	    }
	  }

	  /**
	   * Activate the appropriate bins based on the round definition.
	   * @TODO use the round definition.
	   * @type {Number}
	   */
	  protected void ResetBins(RoundDefinition Round) {
	    for (int i = 0; i < knownBins.Count; i++) {
	      knownBins[i].Reset();
	    }
	  }

	  public void DeactivateBins() {
	    for (int i = 0; i < knownBins.Count; i++) {
	      knownBins[i].Deactivate();
	    }
	  }

	  public void ScoreBin(Collector SubjectBin) {
			_EventSource.TriggerEvent(RoundManager.EVENT_SCOREBIN);

			PresentList BinList = CurrentScoringStrategy.ScoreBin(SubjectBin);

			_ScoreKeeper.AddToScore(BinList);

			if (!BinList.SuccessfulScoring()) {
				_GameManager.AddError();
			}

			_EventSource.TriggerEvent(RoundManager.EVENT_SCOREBIN_AFTER);

	    SubjectBin.Reset();
	  }
	}
}
