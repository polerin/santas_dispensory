public struct GameDefinition {
  // Solo or co-op (for now?)
  public string GameStyle;

  // Easy, Normal, Hard
  public string Difficulty;

  // How long should each round last, if not specified
  public int DefaultTime;


  public int DefaultCoolDown;
  public int AllowedErrors;

  public RoundDefinition[] Rounds;
}
