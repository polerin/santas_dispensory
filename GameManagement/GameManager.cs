using System;
using System.Collections.Generic;
using UnityEngine;
using GameEventBus.Interfaces;

using SMG.Santas.Coordination.Events;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.GameManagement
{
  /// <summary>
  /// Central coordinating object of the game.  Responsible for loading
  /// game definitions and serves as a facade to the game state object.
  /// </summary>
  public class GameManager
  {
    /// <summary>
    /// Base Resource path where the game definition JSON files are stored.
    /// </summary>
    private const string PATH_GAMEDEFINITION = "GameDefinitions/";

    /// <summary>
    /// Default json string returned if a Game Definition can't be loaded.
    /// </summary>
    private const string DEFAULT_GAMEDEF = "{}";

    /// <summary>
    /// Injected Event bus
    /// </summary>
    protected IEventBus<IEvent> _EventBus;

    /// <summary>
    /// Injected list of control sets
    /// </summary>
    protected Dictionary<string, IControlSet> _ControlSets = new Dictionary<string, IControlSet>();

    /// <summary>
    /// The control set currently active
    /// </summary>
    protected IControlSet CurrentControlSet;

    /// <summary>
    /// The game state object.  Created on game definition load
    /// </summary>
    public Game CurrentGame { get; protected set; }

    /// <summary>
    /// Round definition accessor.  Demeter facade.
    /// </summary>
    public RoundDefinition CurrentRound {
      get { return CurrentGame.CurrentRound; }
      protected set { }
    }


    /// <summary>
    /// Constructor injection, starts listening to game and round lifecycle events
    /// </summary>
    /// <param name="EventSource"></param>
    public GameManager(IEventBus<IEvent> EventBus, List<IControlSet> ControlSets)
    {
      _EventBus = EventBus;

      _EventBus.Subscribe<StartSignalEvent>(StartGame);
      _EventBus.Subscribe<MaxErrorsEvent>(EndGame);
      _EventBus.Subscribe<RoundFailedEvent>(EndGame);

      for (int i = 0; i < ControlSets.Count; i++) {
        _ControlSets.Add(ControlSets[i].Slug(), ControlSets[i]);
      }
    }

    /// <summary>
    /// Destructor, clean up our listening.
    /// </summary>
    ~GameManager()
    {
      if (_EventBus == null) {
        return;
      }
      _EventBus.Unsubscribe<StartSignalEvent>(StartGame);
      _EventBus.Unsubscribe<MaxErrorsEvent>(EndGame);
      _EventBus.Unsubscribe<RoundFailedEvent>(EndGame);

    }

    /// <summary>
    /// Check the games state to see if the game is active
    /// @TODO This is pretty crappily named.  Should rename it IsGameOn()
    /// </summary>
    /// <returns></returns>
    public bool GameState()
    {
      if (CurrentGame == null) {
        return false;
      }

      return CurrentGame.gameOn;
    }

    /// <summary>
    /// Add an error to the current game.  If Max errors have been reached send the event.
    /// @TODO This might not be the right home for this method.  Move it all to the Game model?
    /// </summary>
    public void AddError()
    {
      // @TODO figure out how to send back a cancel.  Exception?
      _EventBus.Publish(new ErrorAddedEvent());

      CurrentGame.AddError();

      if (CurrentGame.AtMaxErrors()) {
        // @TODO This should probably live in the Game itself
        _EventBus.Publish(new MaxErrorsEvent());
      }
    }

    /// <summary>
    /// Load a game definition and start the game
    /// </summary>
    protected void StartGame(StartSignalEvent StartEvent)
    {
      if (GameState()) {
        Debug.LogWarning("Attempting to start an already started game");
        return;
      }

      CurrentGame = LoadGameDefinition(StartEvent.GameType);
      ActivateControlSetForGame(CurrentGame);

      CurrentGame.gameOn = true;

      _EventBus.Publish(new GameStartEvent(CurrentGame));

      StartRound();
    }

    /// <summary>
    /// End the currently active game and trigger a scene reset
    /// </summary>
    protected void EndGame(IEvent EndEvent)
    {
      if (!GameState()) {
        Debug.LogWarning("Attempting to End an inactive game");
        return;
      }

      _EventBus.Publish(new GameEndEvent(CurrentGame));
      CurrentGame.gameOn = false;
      DeactivateCurrentControlSet();

      _EventBus.Publish(new GameEndAfterEvent(CurrentGame));
    }

    /// <summary>
    /// Safely activate a known ControlSet
    /// </summary>
    /// <param name="TargetGame"></param>
    protected void ActivateControlSetForGame(Game TargetGame)
    {
      if (CurrentControlSet != null) {
        Debug.LogWarning("Activating new control set without deactivating previous: " + CurrentControlSet.Slug());
        DeactivateCurrentControlSet();
      }

      string controlSetSlug = TargetGame.GameStyle + "ControlSet";

      if (!_ControlSets.TryGetValue(controlSetSlug, out CurrentControlSet)) {
        Debug.LogError("Unable to locate requested control set: " + controlSetSlug);
        return;
      }

      CurrentControlSet.Activate();
    }


    /// <summary>
    /// If a ControlSet has been activated, deactivate it safely
    /// </summary>
    protected void DeactivateCurrentControlSet()
    {
      if (CurrentControlSet != null) {
        CurrentControlSet.Deactivate();
        CurrentControlSet = null;
      }
    }

    /// <summary>
    /// Start a round
    /// @TODO put in some checks to make sure we are not re-starting a round?
    /// </summary>
    protected void StartRound()
    {
      if (!GameState()) {
        Debug.LogWarning("Attempting to Start a round on an inactive game");
        return;
      }

      CurrentGame.AdvanceRound();

      _EventBus.Publish(new RoundStartEvent(CurrentRound));
    }


    /// <summary>
    /// Load a game definition from a resources json file
    /// @TODO this is not currently loading rounds correctly.
    /// @TODO it really seems silly to have this as a one liner. Combine with LoadJson()?
    /// </summary>
    /// <param name="Type"></param>
    /// <returns></returns>
    protected Game LoadGameDefinition(GameTypes Type)
    {
      return JsonUtility.FromJson<Game>(LoadJson(Type));
    }

    /// <summary>
    /// Load a jason file from resources
    /// </summary>
    /// <param name="Type"></param>
    /// <returns></returns>
    protected string LoadJson(GameTypes Type)
    {
      string ResourcePath = PATH_GAMEDEFINITION + Enum.GetName(typeof(GameTypes), Type);
      TextAsset GameJson = (TextAsset)Resources.Load(ResourcePath);

      if (GameJson == null) {
        // throw something here?
        return GameManager.DEFAULT_GAMEDEF;
      }


      return GameJson.text;
    }
  }
}
