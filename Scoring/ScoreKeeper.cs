using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.GameManagement;
using SMG.Santas.Scoring;
using SMG.Santas.ObjectScripts;


namespace SMG.Santas.Scoring
{

  // @TODO Rework this as a "Top Score"
  public class ScoreKeeper
  {

    // UnityAction m_GameStartAction;
    // private UnityAction m_GameEndAction;

    EventSource _EventSource;
    GameManager _GameManager;

    List<int> roundScores = new List<int>();

    private int _Score;
    public int Score {
      get { return _Score; }
      protected set { _Score = value; }
    }

    /**
		 * Constructor
		 */
    public ScoreKeeper(GameManager GameManager, EventSource _EventSource)
    {
      this._GameManager = GameManager;

      // _EventSource.StartListening(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
      // _EventSource.StartListening(GameManager.EVENT_GAMEEND, this.m_GameEndAction);

      // Register our StartGame() with our unity action.
      // this.m_GameStartAction += this.StartGame;
      // this.m_GameEndAction += this.EndGame
    }

    ~ScoreKeeper()
    {
      if (_EventSource == null) {
        return;
      }

      // _EventSource.StopListening(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
      // _EventSource.StopListening(GameManager.EVENT_GAMEEND, this.m_GameEndAction);
    }

    public void AddToScore(PresentList binList)
    {
      AddToScore(binList.Score());
    }

    public void AddToScore(int points)
    {
      Score += points;
      _GameManager.CurrentGame.AddScoreToRound(points);
    }

    public void StartGame()
    {
      Debug.Log("SK StartGame");
      Score = 0;
    }

    public void StartRound(int roundNumber)
    {
      if (!_GameManager.GameState()) {
        return;
      }

      if (roundScores.Count > roundNumber) {
        roundScores[roundNumber] = 0;
      } else {
        roundScores.Insert(roundNumber, 0);
      }

    }
  }
}
