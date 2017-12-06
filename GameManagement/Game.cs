using SMG.Santas.RoundFlow;

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

    public int currentRound = 0;
    public int currentErrors = 0;

    public bool gameOn = false;

    public int AddError() {
      currentErrors++;

      return currentErrors;
    }

    public bool AtMaxErrors() {
      return (currentErrors >= AllowedErrors);
    }
  }
}
