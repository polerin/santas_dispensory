﻿using System.Collections.Generic;
using UnityEngine;
using GameEventBus.Interfaces;

using SMG.Santas.Coordination.Events;
using SMG.Santas.Scoring;
using SMG.Santas.GameManagement;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  public class RoundManager
  {
    public const string EVENT_SCOREBIN = "round_scorebin";
    public const string EVENT_SCOREBIN_AFTER = "round_scorebin_after";

    /// <summary>
    /// Definition for the current round, passes through to _GameManager
    /// </summary>
    public RoundDefinition CurrentRound {
      get { return _GameManager.CurrentRound; }
      protected set { }
    }

    /// <summary>
    /// Current Round Inspector, constructed from round definition
    /// </summary>
    IRoundInspector CurrentRoundInspector;

    /// <summary>
    /// Current Scoring Strategy, constructed from round definition
    /// </summary>
    IScoringStrategy CurrentScoringStrategy;

    /// <summary>
    /// Injected game manager.
    /// </summary>
    GameManager _GameManager;

    /// <summary>
    /// Inject Event bus
    /// </summary>
    IEventBus<IEvent> _EventBus;

    /// <summary>
    /// Injected configuration.  This is used for prefab fetching
    /// </summary>
    Settings _Settings;

    /// <summary>
    /// Default round type, must exist in _RoundInspectors
    /// </summary>
    string defaultRoundType = "binCount";

    /// <summary>
    /// Default scoring strategy, must exist in _ScoringStrategies
    /// </summary>
    string defaultScoringType = "StandardScoring";

    /// <summary>
    /// The index of the current dispenser.  Used by GetNextDispenser()
    /// </summary>
    int currentDispenserIndex = 0;

    /// <summary>
    /// Round inspectors by name, for different round types.
    /// </summary>
    Dictionary<string, IRoundInspector> _RoundInspectors = new Dictionary<string, IRoundInspector>();

    /// <summary>
    /// Scoring strategies by name, for different round types
    /// </summary>
    Dictionary<string, IScoringStrategy> _ScoringStrategies = new Dictionary<string, IScoringStrategy>();

    /// <summary>
    /// List of known present bins.  Filled by Register() calls
    /// </summary>
    List<Collector> knownBins = new List<Collector>();

    /// <summary>
    /// List of known present dispensers.  Filled by Register() calls
    /// </summary>
    List<Dispenser> knownDispensers = new List<Dispenser>();


    /// <summary>
    /// Constructor injection, sets up some game cycle listeners
    /// </summary>
    /// <param name="Manager">Game Manager</param>
    /// <param name="Source">Event Source</param>
    /// <param name="RoundInspectors">List of Round Inspectors, defined in the Installer</param>
    /// <param name="ScoringStrategies">List of Scoring Strategies, defined in the Installer</param>
    /// <param name="Settings">Settings object, sourced in the Installer, set in the inspector</param>
    public RoundManager(
      GameManager Manager,
      IEventBus<IEvent> Source,
      List<IRoundInspector> RoundInspectors,
      List<IScoringStrategy> ScoringStrategies,
      Settings Settings)
    {

      _GameManager = Manager;
      _EventBus = Source;
      _Settings = Settings;

      _EventBus.Subscribe<RoundStartEvent>(StartRound);
      _EventBus.Subscribe<GameEndEvent>(EndRound);
      _EventBus.Subscribe<RoundEndEvent>(EndRound);
      _EventBus.Subscribe<DispenseEvent>(DispenseItem);

      for (int i = 0; i < RoundInspectors.Count; i++) {
        RoundInspectors[i].Inspect(this);
        _RoundInspectors.Add(RoundInspectors[i].Slug(), RoundInspectors[i]);
      }

      for (int i = 0; i < ScoringStrategies.Count; i++) {
        _ScoringStrategies.Add(ScoringStrategies[i].Slug(), ScoringStrategies[i]);
      }
    }

    /// <summary>
    /// Destructor, unregister listeners if the IEventBus<IEvent> still exists.
    /// </summary>
    ~RoundManager()
    {
      if (_EventBus == null) {
        return;
      }

      _EventBus.Unsubscribe<RoundStartEvent>(StartRound);
      _EventBus.Unsubscribe<GameEndEvent>(EndRound);
      _EventBus.Unsubscribe<RoundEndEvent>(EndRound);
      _EventBus.Unsubscribe<DispenseEvent>(DispenseItem);
    }

    /// <summary>
    /// Register a collection bin so we can manage it later
    /// </summary>
    /// <param name="subjectBin"></param>
    public void Register(Collector subjectBin)
    {
      if (!knownBins.Contains(subjectBin)) {
        knownBins.Add(subjectBin);
      }
    }


    /// <summary>
    /// Register a present dispenser so we can manage it later
    /// </summary>
    /// <param name="subjectDispenser"></param>
    public void Register(Dispenser subjectDispenser)
    {
      if (!knownDispensers.Contains(subjectDispenser)) {
        knownDispensers.Add(subjectDispenser);
      }
    }

    
    /// <summary>
    /// Load up the right round inspector, activate the appropriate bins and dispensers.
    /// @TODO when I have event params figured out, use the passed round definition.
    /// </summary>
    protected void StartRound(RoundStartEvent StartEvent)
    {
      if (!_GameManager.GameState()) {
        Debug.LogWarning("Attempting to start a round for a game that is not started");
        return;
      }
      RoundDefinition CurrentRound = StartEvent.Round;

      SetRoundInspector(CurrentRound);
      SetScoringStrategy(CurrentRound);
      ResetBins(CurrentRound);
      ActivateDispensers(CurrentRound);
    }

    /// <summary>
    /// End the current round, deactivating bins and dispensers.
    /// </summary>
    protected void EndRound(IEvent Event)
    {
      if (!_GameManager.GameState()) {
        return;
      }

      DeactivateBins();
      DeactivateDispensers();
    }

    /// <summary>
    /// Set the current Round Inspector based on the supplied round definition
    /// @TODO exception for default round manager not found
    /// </summary>
    /// <param name="Round"></param>
    protected void SetRoundInspector(RoundDefinition Round)
    {
      IRoundInspector TargetInspector = null;

      if (_RoundInspectors.TryGetValue(CurrentRound.roundType, out TargetInspector)) {
        CurrentRoundInspector = TargetInspector;
      } else {
        Debug.LogWarning("Round type not found in inspector list: " + CurrentRound.roundType);
        CurrentRoundInspector = _RoundInspectors[defaultRoundType];
      }

      CurrentRoundInspector.Activate();
    }

    /// <summary>
    /// Set the current Scoring Strategy based on the supplied round definition 
    /// </summary>
    /// <param name="Round"></param>
    protected void SetScoringStrategy(RoundDefinition Round)
    {
      IScoringStrategy TargetStrategy = null;

      if (_ScoringStrategies.TryGetValue(Round.scoreType, out TargetStrategy)) {
        CurrentScoringStrategy = TargetStrategy;
      } else {
        Debug.LogWarning("Scoring type not found in strategy list: " + Round.scoreType);
        CurrentScoringStrategy = _ScoringStrategies[defaultScoringType];
      }
    }

     
	 
    /// <summary>
    /// Activate the dispensers based on the supplied round definition
    /// @TODO Err.. use the supplied round definition.
    /// </summary>
    /// <param name="Round"></param>
    protected void ActivateDispensers(RoundDefinition Round)
    {
      for (int i = 0; i < knownDispensers.Count; i++) {
        knownDispensers[i].Activate();
      }
    }

    /// <summary>
    /// Activate the appropriate bins based on the round definition
    /// This will generate a new present list in the bins
    /// @TODO use the supplied round definition.
    /// </summary>
    /// <param name="Round"></param>
    protected void ResetBins(RoundDefinition Round)
    {
      for (int i = 0; i < knownBins.Count; i++) {
        knownBins[i].Reset();
      }
    }

    /// <summary>
    /// Deactivate all registered bins
    /// </summary>
    public void DeactivateBins()
    {
      for (int i = 0; i < knownBins.Count; i++) {
        knownBins[i].Deactivate();
      }
    }

    /// <summary>
    /// Deactivate all registered dispensers
    /// </summary>
    public void DeactivateDispensers()
    {
      for (int i = 0; i < knownDispensers.Count; i++) {
        knownDispensers[i].Deactivate();
      }
    }

    /// <summary>
    /// Score the supplied bin
    /// @TODO This is a bit funky, come back to it.
    /// </summary>
    /// <param name="SubjectBin"></param>
    public void ScoreBin(Collector SubjectBin)
    {
      PresentList BinList = CurrentScoringStrategy.ScoreBin(SubjectBin);

      _GameManager.CurrentGame.AddScore(BinList);

      if (!BinList.SuccessfulScoring()) {
        _GameManager.AddError();
      }

      _EventBus.Publish(new ScoreBinEvent(SubjectBin, _GameManager.CurrentRound));

      SubjectBin.Reset();
    }

    /**
		 * All of this is annoying as heck,  Come back to it later with delegates I guess.
     * These methods are individual implementations as I was stuck on the event bus with params.
		 */ 

    public void DispenseItem(DispenseEvent DispenseEvent)
    {
      CatchTypes SubjectType = DispenseEvent.CatchType;

      switch (SubjectType) {
        case CatchTypes.Bear:
          GetNextDispenser().DispenseItem(_Settings.BearPrefab);
          break;
       case CatchTypes.Ball: 
          GetNextDispenser().DispenseItem(_Settings.BallPrefab);
          break;

       case CatchTypes.Present: 
          GetNextDispenser().DispenseItem(_Settings.PresentPrefab);
          break;

       case CatchTypes.Horse: 
          GetNextDispenser().DispenseItem(_Settings.HorsePrefab);
          break;
      }
    }



    /// <summary>
    /// Find the next dispenser in the list
    /// </summary>
    /// <returns></returns>
    public Dispenser GetNextDispenser()
    {
      currentDispenserIndex++;
      if (currentDispenserIndex == knownDispensers.Count) {
        currentDispenserIndex = 0;
      }

      return knownDispensers[currentDispenserIndex];
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
