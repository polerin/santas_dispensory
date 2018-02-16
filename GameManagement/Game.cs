using SMG.Santas.RoundFlow;
using SMG.Santas.Scoring;

namespace SMG.Santas.GameManagement
{
  // @TODO Serializable
  // @TODO WHY IS EVERYTHING PUBLIC OH MY GOD
  /// <summary>
  /// This is the main model for game data.  It contains score,
  /// errors, the round list, and any other information needed
  /// to represent the progress of the player(s) in the game.
  /// </summary>
  public class Game
  {
    public const string GAMESTYLE_PARTNER = "partner";
    public const string GAMESTYLE_SOLO = "solo";
    // Solo or co-op (for now?)
    public string GameStyle = "partner";

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

    protected GameDescriptor Description;

    public Game()
    {
      Description = new GameDescriptor();
    }

    public int AddError()
    {
      currentErrors++;

      return currentErrors;
    }

    public bool AtMaxErrors()
    {
      return (currentErrors >= AllowedErrors);
    }

    public int AddScore(ScoreResult Score)
    {
      return AddScore(Score.scoreChange);
    }

    public int AddScore(int points)
    {
      return currentScore += points;
    }

    public int AddScoreToRound(int score)
    {
      return AddScoreToRound(score, currentRoundIndex);
    }

    // @TODO rounds currently don't have scores, need to implement
    public int AddScoreToRound(int score, int round)
    {
      return 0;
    }

    public void AddBin()
    {
      CurrentRound.binCount++;
    }

    // @todo make this not a nasty stub.
    public RoundDefinition AdvanceRound()
    {
      CurrentRound = GenerateStubRoundDefinition();
      return CurrentRound;
    }

    public GameDescriptor GetDescription()
    {
      Description.round = currentRoundIndex + 1;
      Description.score = currentScore;
      Description.errors = currentErrors;

      return Description;
    }

    protected RoundDefinition GenerateStubRoundDefinition()
    {
      RoundDefinition Stub = new RoundDefinition();
      Stub.roundType = "binCount";
      Stub.scoreType = "StandardScoring";
      Stub.bins = new bool[] { true, true };
      Stub.dispensers = new bool[] { false, true, true, false, false };
      Stub.maxBins = 1;

      return Stub;
    }
  }
}
