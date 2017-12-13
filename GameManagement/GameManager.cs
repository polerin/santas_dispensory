using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.Scoring;
using SMG.Santas.RoundFlow;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.GameManagement {
	public class GameManager {
		// Issued to start the game.  (not issued by game manager)
		public const string EVENT_GAMESTART = "game_start";

		// Issued to signal that the game has just started
		public const string EVENT_GAMESTART_AFTER = "game_start_after";

	  // Issued to signal that the game is ending
		public const string EVENT_GAMEEND = "game_end";

		//Issued to signal that the game has just ended
		public const string EVENT_GAMEEND_AFTER = "game_end_after";

	  // Notification that a round is about to start
	  public const string EVENT_ROUNDSTART = "round_start";

	  // Notification that a round is about to end
	  public const string EVENT_ROUNDEND = "round_end";

	  // Notification that an error is about to be added.
	  public const string EVENT_ERRORADDED = "round_error_add";

	  // Notification that the player has hit the maximum error count
	  public const string EVENT_MAXERRORS = "round_max_errors";

		// Base Resource path where the game definition JSON files are stored.
	  private const string PATH_GAMEDEFINITION = "GameDefinitions/";

		// Default json string returned if a Game Definition can't be loaded.
	  private const string DEFAULT_GAMEDEF = "{}";

		private UnityAction m_GameStartAction;
		private UnityAction m_MaxErrorsAction;

		protected ScoreboardDisplay ScoreBoard;
		protected EventSource _EventSource;

		public Game CurrentGame { get; protected set; }
		public RoundDefinition CurrentRound { get; protected set; }

		/**
		 * Constructor
		 */
		public GameManager (EventSource EventSource) {
			InitMonitors();
			_EventSource = EventSource;

			// Register our StartGame() with our unity action.
			m_GameStartAction += StartGame;

			// Maybe later we need to have some more logic here, but not for now
			// m_MaxErrorsAction += EndGame;

			// _EventSource.StartListening(GameManager.EVENT_GAMESTART, m_GameStartAction);
			// _EventSource.StartListening(GameManager.EVENT_MAXERRORS, m_MaxErrorsAction);
			_EventSource.StartListening(GameManager.EVENT_GAMESTART, StartGame);
			_EventSource.StartListening(GameManager.EVENT_MAXERRORS, EndGame);
			_EventSource.StartListening(AbstractRoundInspector.EVENT_CONDITION_SUCCESS, EndRound);
			_EventSource.StartListening(AbstractRoundInspector.EVENT_CONDITION_FAILURE, EndGame);


		}

	  ~GameManager() {
			// _EventSource.StopListening(GameManager.EVENT_GAMESTART, m_GameStartAction);
		}


	  public bool GameState() {
			if (CurrentGame == null) {
				return false;
			}

	    return CurrentGame.gameOn;
	  }

	  public void AddError() {
	    // @TODO figure out how to send back a cancel.  Exception?
	    _EventSource.TriggerEvent(GameManager.EVENT_ERRORADDED);

	    CurrentGame.AddError();

	    if (CurrentGame.AtMaxErrors()) {
	      _EventSource.TriggerEvent(GameManager.EVENT_MAXERRORS);
	    }
	  }

		/* Game Lifecycle event management */

		protected void StartGame() {
			Debug.Log("GM Startgame");
			if (GameState()) {
				Debug.Log("Attempting to start an already started game");
				return;
			}

			CurrentGame = LoadGameDefinition(GameTypes.StandardNormal);
			CurrentGame.gameOn = true;
Debug.Log(JsonUtility.ToJson(CurrentGame));
			_EventSource.TriggerEvent(GameManager.EVENT_GAMESTART_AFTER);

			// StartRound();
		}

	  protected void EndGame() {
			if (!GameState()) {
				Debug.Log("Attempting to End an inactive game");
				return;
			}

			_EventSource.TriggerEvent(GameManager.EVENT_GAMEEND);
	    CurrentGame.gameOn = false;

			_EventSource.TriggerEvent(GameManager.EVENT_GAMEEND_AFTER);
		}

		protected void StartRound() {
	    if (!GameState()) {
	      return;
	    }

			CurrentGame.AdvanceRound();

	    _EventSource.TriggerEvent(GameManager.EVENT_ROUNDSTART);
		}

		protected void EndRound() {
	    _EventSource.TriggerEvent(GameManager.EVENT_ROUNDEND);
		}


		/* Mechanics and loading methods. */

		protected void InitMonitors() {
			if (Display.displays.Length <= 1) {
				Debug.Log("Well that's odd, no real monitor");
				return;
			}

			Display.displays[1].Activate();
		}

		protected Game LoadGameDefinition(GameTypes Type) {
	    return JsonUtility.FromJson<Game>(LoadJson(Type));
	  }

	  protected string LoadJson(GameTypes Type) {
			string ResourcePath = GameManager.PATH_GAMEDEFINITION + Enum.GetName(typeof(GameTypes), Type);
	    TextAsset GameJson = (TextAsset) Resources.Load(ResourcePath);

	    if (GameJson == null) {
	      // throw something here?
	      return GameManager.DEFAULT_GAMEDEF;
	    }


	    return GameJson.text;
	  }
	}
}
