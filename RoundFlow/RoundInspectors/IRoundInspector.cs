namespace SMG.Santas.RoundFlow {
  public interface IRoundInspector {
      string Slug();
      void Inspect(RoundManager Manager);
      void Activate();
      void Deactivate();
  }
}
