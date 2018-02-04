using UnityEngine;
using UnityEngine.UI;


namespace SMG.Santas.ObjectScripts
{

  public class ScoreboardDisplay : MonoBehaviour
  {
    [SerializeField,  Tooltip("The Canvas displayed before and after the game")]
    Canvas FullCanvas;

    [SerializeField, Tooltip("The Canvas displayed while the game is active")]
    Canvas DetailCanvas;

    [SerializeField, Tooltip("The default contents for the FullCanvas text object")]
    string DefaultFullText = "Pull the lever to start the game!";

    Text FullText;

    Text ScoreText;
    Text ScoreValue;

    Text RoundText;
    Text RoundValue;

    Text ErrorsText;
    Text ErrorsValue;

    Text DetailText;
    Text DetailValue;


    // Use this for initialization
    void Start()
    {
      FullCanvas.enabled = true;
      DetailCanvas.enabled = false;
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
