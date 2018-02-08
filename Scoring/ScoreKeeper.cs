using System.Collections.Generic;
using UnityEngine;
using GameEventBus.Interfaces;

using SMG.Santas.GameManagement;


namespace SMG.Santas.Scoring
{

  // @TODO Rework this as a "Top Score"
  public class ScoreKeeper
  {

    // UnityAction m_GameStartAction;
    // private UnityAction m_GameEndAction;

    IEventBus<IEvent> _EventBus;
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
    public ScoreKeeper(GameManager GameManager, IEventBus<IEvent> _EventBus)
    {
      this._GameManager = GameManager;

      // _EventBus.Subscribe(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
      // _EventBus.Subscribe(GameManager.EVENT_GAMEEND, this.m_GameEndAction);

      // Register our StartGame() with our unity action.
      // this.m_GameStartAction += this.StartGame;
      // this.m_GameEndAction += this.EndGame
    }

    ~ScoreKeeper()
    {
      if (_EventBus == null) {
        return;
      }

      // _EventBus.Unsubscribe(GameManager.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
      // _EventBus.Unsubscribe(GameManager.EVENT_GAMEEND, this.m_GameEndAction);
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
