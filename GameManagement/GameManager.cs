using System;
using UnityEngine;

using SMG.Coordination;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.GameManagement
{
  /// <summary>
  /// Central coordinating object of the game.  Responsible for loading
  /// game definitions and serves as a facade to the game state object.
  /// </summary>
  public class GameManager
  {
    /// <summary>Issued to start the game.  (not issued by game manager)</summary>
    public const string EVENT_GAMESTART = "game_start";

    /// <summary>Issued to signal that the game has just started</summary>    
    public const string EVENT_GAMESTART_AFTER = "game_start_after";

    /// <summary>Issued to signal that the game has just started</summary>
    public const string EVENT_GAMEEND = "game_end";

    /// <summary>Issued to signal that the game has just ended</summary>
    public const string EVENT_GAMEEND_AFTER = "game_end_after";

    /// <summary>Notification that a round is about to start</summary>
    public const string EVENT_ROUNDSTART = "round_start";

    /// <summary>Notification that a round is about to end</summary>
    public const string EVENT_ROUNDEND = "round_end";

    /// <summary>Notification that an error is about to be added.</summary>
    public const string EVENT_ERRORADDED = "round_error_add";

    /// <summary>Notification that the player has hit the maximum error count</summary>
    public const string EVENT_MAXERRORS = "round_max_errors";

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
    protected EventSource _EventSource;

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
    public GameManager(EventSource EventSource)
    {
      InitMonitors();
      _EventSource = EventSource;

      _EventSource.StartListening(GameManager.EVENT_GAMESTART, StartGame);
      _EventSource.StartListening(GameManager.EVENT_MAXERRORS, EndGame);
      _EventSource.StartListening(AbstractRoundInspector.EVENT_CONDITION_SUCCESS, EndRound);
      _EventSource.StartListening(AbstractRoundInspector.EVENT_CONDITION_FAILURE, EndGame);


    }

    /// <summary>
    /// Destructor, clean up our listening.
    /// </summary>
    ~GameManager()
    {
      if (_EventSource == null) {
        return;
      }

      _EventSource.StopListening(GameManager.EVENT_GAMESTART, StartGame);
      _EventSource.StopListening(GameManager.EVENT_MAXERRORS, EndGame);
      _EventSource.StopListening(AbstractRoundInspector.EVENT_CONDITION_SUCCESS, EndRound);
      _EventSource.StopListening(AbstractRoundInspector.EVENT_CONDITION_FAILURE, EndGame);
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
    /// </summary>
    public void AddError()
    {
      // @TODO figure out how to send back a cancel.  Exception?
      _EventSource.TriggerEvent(GameManager.EVENT_ERRORADDED);

      CurrentGame.AddError();

      if (CurrentGame.AtMaxErrors()) {
        _EventSource.TriggerEvent(GameManager.EVENT_MAXERRORS);
      }
    }

    /// <summary>
    /// Load a game definition and start the game
    /// @TODO provide a way to load different defintions
    /// </summary>
    protected void StartGame()
    {
      if (GameState()) {
        Debug.LogWarning("Attempting to start an already started game");
        return;
      }

      CurrentGame = LoadGameDefinition(GameTypes.StandardNormal);
      CurrentGame.gameOn = true;

      _EventSource.TriggerEvent(GameManager.EVENT_GAMESTART_AFTER);

      StartRound();
    }

    /// <summary>
    /// End the currently active game and trigger a scene reset
    /// </summary>
    protected void EndGame()
    {
      if (!GameState()) {
        Debug.LogWarning("Attempting to End an inactive game");
        return;
      }

      _EventSource.TriggerEvent(GameManager.EVENT_GAMEEND);
      CurrentGame.gameOn = false;

      _EventSource.TriggerEvent(GameManager.EVENT_GAMEEND_AFTER);
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

      _EventSource.TriggerEvent(GameManager.EVENT_ROUNDSTART);
    }

    /// <summary>
    /// End the currently active round
    /// </summary>
    protected void EndRound()
    {
      _EventSource.TriggerEvent(GameManager.EVENT_ROUNDEND);
    }

    /// <summary>
    /// Activate a display on the "normal" monitor for the seated players
    /// @TODO this currently doesn't work.  What am I doing wrong?
    /// </summary>
    protected void InitMonitors()
    {
      if (Display.displays.Length <= 1) {
        Debug.Log("Well that's odd, no real monitor");
        return;
      }

      Display.displays[1].Activate();
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
      string ResourcePath = GameManager.PATH_GAMEDEFINITION + Enum.GetName(typeof(GameTypes), Type);
      TextAsset GameJson = (TextAsset)Resources.Load(ResourcePath);

      if (GameJson == null) {
        // throw something here?
        return GameManager.DEFAULT_GAMEDEF;
      }


      return GameJson.text;
    }
  }
}
