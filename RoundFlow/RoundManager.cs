using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.Scoring;
using SMG.Santas.RoundFlow;
using SMG.Santas.GameManagement;
using SMG.Santas.ObjectScripts;
using SMG.Santas.Events;

namespace SMG.Santas.RoundFlow
{
  public class RoundManager
  {
    public const string EVENT_SCOREBIN = "round_scorebin";
    public const string EVENT_SCOREBIN_AFTER = "round_scorebin_after";

    UnityAction<int> DispenseAction;

    public RoundDefinition CurrentRound {
      get { return _GameManager.CurrentRound; }
      protected set { }
    }

    IRoundInspector CurrentRoundInspector;
    IScoringStrategy CurrentScoringStrategy;
    GameManager _GameManager;
    EventSource _EventSource;
    Settings _Settings;

    string defaultRoundType = "binCount";
    string defaultScoringType = "StandardScoring";

    Dictionary<string, IRoundInspector> _RoundInspectors = new Dictionary<string, IRoundInspector>();
    Dictionary<string, IScoringStrategy> _ScoringStrategies = new Dictionary<string, IScoringStrategy>();
    List<Collector> knownBins = new List<Collector>();
    List<Dispenser> knownDispensers = new List<Dispenser>();

    /**
	   * Constructor
	   */
    public RoundManager(
      GameManager Manager,
      EventSource Source,
      List<IRoundInspector> RoundInspectors,
      List<IScoringStrategy> ScoringStrategies,
      Settings Settings)
    {

      _GameManager = Manager;
      _EventSource = Source;
      _Settings = Settings;

      _EventSource.StartListening(GameManager.EVENT_ROUNDSTART, StartRound);
      _EventSource.StartListening(GameManager.EVENT_GAMEEND, EndRound);
      _EventSource.StartListening(GameManager.EVENT_ROUNDEND, EndRound);

      _EventSource.StartListening(Dispenser.DISPENSE_BEAR, DispenseBear);
      _EventSource.StartListening(Dispenser.DISPENSE_BALL, DispenseBall);
      _EventSource.StartListening(Dispenser.DISPENSE_PRESENT, DispensePresent);
      _EventSource.StartListening(Dispenser.DISPENSE_HORSE, DispenseHorse);

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
    ~RoundManager()
    {
      Debug.Log("Roundmanager destructor");
      if (_EventSource == null) {
        return;
      }

      _EventSource.StopListening(GameManager.EVENT_ROUNDSTART, StartRound);
      _EventSource.StopListening(GameManager.EVENT_GAMEEND, EndRound);
      _EventSource.StopListening(GameManager.EVENT_ROUNDEND, EndRound);

      _EventSource.StopListening(Dispenser.DISPENSE_BEAR, DispenseBear);
      _EventSource.StopListening(Dispenser.DISPENSE_BEAR, DispenseBall);
      _EventSource.StopListening(Dispenser.DISPENSE_BEAR, DispensePresent);
      _EventSource.StopListening(Dispenser.DISPENSE_BEAR, DispenseHorse);
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
    protected void StartRound()
    {
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

    protected void EndRound()
    {
      if (!_GameManager.GameState()) {
        return;
      }

      DeactivateBins();
    }

    // @TODO exception for default round manager not found
    protected void SetRoundManager(RoundDefinition Round)
    {
      IRoundInspector TargetInspector = null;

      if (_RoundInspectors.TryGetValue(CurrentRound.roundType, out TargetInspector)) {
        CurrentRoundInspector = TargetInspector;
      } else {
        Debug.Log("Round type not found in inspector list: " + CurrentRound.roundType);
        CurrentRoundInspector = _RoundInspectors[defaultRoundType];
      }

      CurrentRoundInspector.Activate();
    }

    protected void SetScoringStrategy(RoundDefinition Round)
    {
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
    protected void ActivateDispensers(RoundDefinition Round)
    {
      for (int i = 0; i < knownDispensers.Count; i++) {
        knownDispensers[i].Activate();
      }
    }

    /**
	   * Activate the appropriate bins based on the round definition.
	   * @TODO use the round definition.
	   * @type {Number}
	   */
    protected void ResetBins(RoundDefinition Round)
    {
      for (int i = 0; i < knownBins.Count; i++) {
        knownBins[i].Reset();
      }
    }

    public void DeactivateBins()
    {
      for (int i = 0; i < knownBins.Count; i++) {
        knownBins[i].Deactivate();
      }
    }

    public void ScoreBin(Collector SubjectBin)
    {
      _EventSource.TriggerEvent(RoundManager.EVENT_SCOREBIN);

      PresentList BinList = CurrentScoringStrategy.ScoreBin(SubjectBin);

      _GameManager.CurrentGame.AddScore(BinList);

      if (!BinList.SuccessfulScoring()) {
        _GameManager.AddError();
      }

      _EventSource.TriggerEvent(RoundManager.EVENT_SCOREBIN_AFTER);

      SubjectBin.Reset();
    }

    /**
		 *  All of this is annoying as heck,  Come back to it later with delegates I guess.
		 */

    public void DispenseBear()
    {
      GetNextDispenser().DispenseItem(_Settings.BearPrefab);
    }

    public void DispenseBall()
    {
      GetNextDispenser().DispenseItem(_Settings.BallPrefab);
    }

    public void DispensePresent()
    {
      GetNextDispenser().DispenseItem(_Settings.PresentPrefab);
    }

    public void DispenseHorse()
    {
      GetNextDispenser().DispenseItem(_Settings.HorsePrefab);
    }

    public Dispenser GetNextDispenser()
    {
      return knownDispensers[1];
    }

    [System.Serializable]
    public class Settings
    {
      // @TODO Booo hardcode but whatever.
      public GameObject BearPrefab;
      public GameObject BallPrefab;
      public GameObject PresentPrefab;
      public GameObject HorsePrefab;
    }
  }
}
