using UnityEngine;
using UnityEngine.UI;

using SMG.Santas.GameManagement;

namespace SMG.Santas.ObjectScripts
{
  /// <summary>
  /// @TODO Use tagging or something to auto populate the text and value fields?
  /// @TODO Add injection of EventSource and listen for game start to display the detail canvas.
  /// </summary>
  public class ScoreboardDisplay : MonoBehaviour
  {
    [SerializeField, Tooltip("The default string to be displayed on the FullCanvas text object")]
    string defaultFullText = "Pull the lever to start the game!";

    [SerializeField,  Tooltip("The Canvas displayed before and after the game, takes up the entire width of the scoreboard.")]
    Canvas FullCanvas;

    /// <summary>
    /// The UI object for text that takes up the entire width of the scoreboard.
    /// </summary>
    [SerializeField]
    Text FullText;

    [SerializeField, Tooltip("The Canvas displayed while the game is active")]
    Canvas DetailCanvas;

    [SerializeField, Tooltip("The Score label text object, which should have an Text child object to use as it's value display.")]
    Text ScoreText;
    [SerializeField]
    Text ScoreValue;

    [SerializeField, Tooltip("The Round label text object, which should have an Text child object to use as it's value display.")]
    Text RoundText;
    [SerializeField]
    Text RoundValue;

    [SerializeField, Tooltip("The Errors label text object, which should have an Text child object to use as it's value display.")]
    Text ErrorsText;
    [SerializeField]
    Text ErrorsValue;

    [SerializeField, Tooltip("The per-inspector criteria label, which should have an Text child object to use as it's value display.")]
    Text DetailText;
    [SerializeField]
    Text DetailValue;


    // Use this for initialization
    void Start()
    {
      DisplayFullText(defaultFullText);
      DetailCanvas.enabled = false;
    }

    protected void DisplayFullText()
    {
      DetailCanvas.enabled = false;
      FullCanvas.enabled = false;
    }

    protected void DisplayFullText(string newText)
    {
      if (FullText != null) {
        FullText.text = newText;
      }
  
      DisplayFullText();
    }

    protected void DisplayDetails()
    {
      FullCanvas.enabled = false;
      DetailCanvas.enabled = true;
    }

    public void RefreshDetails(GameDescriptor Game)
    {
      RoundValue.text = Game.round.ToString();
      ErrorsValue.text = Game.errors.ToString();
      ScoreValue.text = Game.score.ToString();
      DetailText.text = Game.detailLabel;
      DetailValue.text = Game.detailValue;
 
    }


    public void UpdateFullText(int bins, int errors, int score, int round, bool gameOn)
    {
      this.ScoreText.text = "Bins Left:\t" + bins + "\n"
        + "Errors:\t" + errors + "\n"
        + "Score:\t" + score + "\n"
        + "Round:\t" + (round + 1) + "\n"
        + "\nGame:\t";
      this.ScoreText.text += (gameOn) ? "On" : "Off";
    }
  }
}
