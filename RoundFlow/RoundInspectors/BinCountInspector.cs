namespace SMG.Santas.Roundflow {

  public class BinCountInspector : IRoundInspector {
    private EventSource _EventSource;

    public BinCountInspector (EventSource Source) {
      this._EventSource = Source;

      this._EventSource.
    }

    ~BinCountInspector () {
      _EventSource.StartListening(ScoreKeeper.EVENT_GAMESTART_AFTER, this.m_GameStartAction);
    }
  }
}
