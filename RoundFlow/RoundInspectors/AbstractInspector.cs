using UnityEngine.Events;

using SMG.Coordination;
using SMG.Santas.RoundFlow;
using SMG.Santas.Scoring;
using SMG.Santas.GameManagement;

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

    public AbstractRoundInspector(EventSource Source, GameManager GameManager)
    {
      this._EventSource = Source;
      this._GameManager = GameManager;
    }


    public void Inspect(RoundManager Manager)
    {
      this._RoundManager = Manager;
    }

    public abstract string Slug();
    public abstract void Activate();
    public abstract void Deactivate();

  }
}
