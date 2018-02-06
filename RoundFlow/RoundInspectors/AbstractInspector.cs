using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.GameManagement;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  public abstract class AbstractRoundInspector : IRoundInspector
  {
    public const string EVENT_CONDITION_SUCCESS = "inspector sees success";
    public const string EVENT_CONDITION_FAILURE = "inspector sees failure";

    protected UnityAction m_RoundStartAction;
    protected UnityAction m_RoundEndAction;
    protected UnityAction m_ScoreAction;
    protected UnityAction m_ErrorAction;

    protected EventSource _EventSource;
    protected RoundManager _RoundManager;
    protected GameManager _GameManager;
    protected ScoreboardDisplay _Scoreboard;


    /// <summary>
    /// This text will be used to describe the custom detail label displayed
    /// by the scoreboard, if any.
    /// </summary>
    protected string DetailLabel;

    /// <summary>
    /// This text will be used to describe the custom detail value displayed 
    /// by the scoreboard, if any.
    /// </summary>
    protected string DetailValue;

    public AbstractRoundInspector(EventSource Source, GameManager GameManager, ScoreboardDisplay Scoreboard)
    {
      this._EventSource = Source;
      this._GameManager = GameManager;
      this._Scoreboard = Scoreboard;
    }


    /// <summary>
    /// This is the entry point for the inspector.
    /// </summary>
    /// <param name="Manager"></param>
    public void Inspect(RoundManager Manager)
    {
      this._RoundManager = Manager;
    }

    /// <summary>
    /// Update the scoreboard with the current game state.
    /// @TODO this is funky, redo when event system can pass details.
    /// </summary>
    protected void UpdateScoreboard()
    {
      _Scoreboard.RefreshDetails(GetGameDescription());
    }

    /// <summary>
    /// Build a GameDescriptor with the current state,
    /// including any custom detail label and value.
    /// </summary>
    /// <returns></returns>
    protected GameDescriptor GetGameDescription()
    {
      GameDescriptor Game;
      if (_GameManager.GameState()) {
        Game = _GameManager.CurrentGame.GetDescription();
      } else {
        // Game hasn't started yet, just being careful
        Game = new GameDescriptor();
      }

      Game.detailLabel = GetDetailLabel();
      Game.detailValue = GetDetailValue();

      return Game;
    }

    /// <summary>
    /// Template method, override to return a custom detail
    /// label for the scoreboard to display.
    /// </summary>
    /// <returns></returns>
    protected string GetDetailLabel()
    {
      return "";
    }

    /// <summary>
    /// Template method, override to return a custom detail
    /// value for the scoreboard to display.
    /// </summary>
    /// <returns></returns>
    protected string GetDetailValue()
    {
      return "";
    }

    public abstract string Slug();
    public abstract void Activate();
    public abstract void Deactivate();

  }
}
