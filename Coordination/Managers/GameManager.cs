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
	private UnityAction m_MaxErrorsAction;

	protected ScoreboardDisplay ScoreBoard;
	protected EventSource _EventSource;

	/**
	 * Constructor
	 */
	public GameManager (EventSource EventSource) {
		InitMonitors();
		_EventSource = EventSource;

		_EventSource.StartListening(GameManager.EVENT_GAMESTART, m_GameStartAction);
		_EventSource.StartListening(RoundManager.EVENT_MAXERRORS, m_MaxErrorsAction);

		// Register our StartGame() with our unity action.
		m_GameStartAction += StartGame;

		// Maybe later we need to have some more logic here, but not for now
		m_MaxErrorsAction += EndGame;
	}

  ~GameManager() {
		_EventSource.StopListening(GameManager.EVENT_GAMESTART, m_GameStartAction);
	}

	protected void InitMonitors() {
		if (Display.displays.Length <= 1) {
			Debug.Log("Well that's odd, no real monitor");
			return;
		}

		Display.displays[1].Activate();
	}

  public bool GameState() {
    return gameOn;
  }

	void StartGame() {
		gameOn = true;
		_EventSource.TriggerEvent(GameManager.EVENT_GAMESTART_AFTER);
	}

  void EndGame() {
		_EventSource.TriggerEvent(GameManager.EVENT_GAMEEND);
    gameOn = false;
		_EventSource.TriggerEvent(GameManager.EVENT_GAMEEND_AFTER);
	}

}
