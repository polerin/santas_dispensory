using SMG.Santas.RoundFlow;
using SMG.Santas.Scoring;

namespace SMG.Santas.GameManagement {
  //@TODO Serializable
  public class Game {
    // Solo or co-op (for now?)
    public string GameStyle = "co-op";

    // Easy, Normal, Hard
    public string Difficulty = "normal";

    // How long should each round last, if not specified
    public int DefaultTime = 60;


    public int DefaultCoolDown = 3;
    public int AllowedErrors = 3;

    public RoundDefinition[] Rounds;

    public RoundDefinition CurrentRound;

    public int currentRoundIndex = 0;
    public int currentErrors = 0;
    public int currentScore = 0;

    public bool gameOn = false;

    public int AddError() {
      currentErrors++;

      return currentErrors;
    }

    public bool AtMaxErrors() {
      return (currentErrors >= AllowedErrors);
    }

    public int AddScore(PresentList binList) {
      return AddScore(binList.Score());
    }

    public int AddScore(int points) {
      return currentScore += points;
    }

    public int AddScoreToRound(int score) {
      return AddScoreToRound(score, currentRoundIndex);
    }

    // @TODO rounds currently don't have scores, need to implement
    public int AddScoreToRound(int score, int round) {
      return 0;
    }

    // @todo make this not a nasty stub.
    public Game AdvanceRound() {
      CurrentRound = GenerateStubRoundDefinition();
      return this;
    }

    protected RoundDefinition GenerateStubRoundDefinition() {
      RoundDefinition Stub = new RoundDefinition();
      Stub.roundType = "binCount";
      Stub.scoreType = "StandardScoring";
      Stub.bins = new bool[] {true, true};
      Stub.dispensers = new bool[] {false, true, true, false, false};
      Stub.maxBins = 5;

      return Stub;
    }
  }
}
