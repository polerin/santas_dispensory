/*
  This is really feeling as if it should be a class instead of a struct, with some of the game manager's event managment in it.
 */
namespace SMG.Santas.GameMana

public Class Game {
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
}
