using SMG.Coordination;
using SMG.Santas.RoundFlow.RoundInspectors;

namespace SMG.Santas.Roundflow.RoundInspectors {
  public class BinCountInspector : IRoundInspector {
    private EventSource _EventSource;

    public BinCountInspector (EventSource Source) {
      this._EventSource = Source;

      }

    ~BinCountInspector () {
      _EventSource.StartListening(ScoreKeeper.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
    }
  }
}
