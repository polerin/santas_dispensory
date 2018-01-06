namespace SMG.Santas.RoundFlow
{
  public struct RoundDefinition
  {
    public string roundType;
    public string scoreType;
    public bool[] bins;
    public bool[] dispensers;

    public int binCount;
    public int errorCount;
    public int score;

    public int maxTime;
    public int maxBins;
    public int maxScore;
    public int maxErrors;
  }
}
