using UnityEngine;
using UnityEngine.UI;


namespace SMG.Santas.ObjectScripts
{
  // @ todo I HATE THIS, use named params with defaults
  public class ScoreboardDisplay : MonoBehaviour
  {
    [SerializeField] GameObject ScoreTextObject;
    Text ScoreText;

    // Use this for initialization
    void Start()
    {
      this.ScoreText = ScoreTextObject.GetComponent<Text>();
    }

    public void UpdateText(int bins, int errors, int score, int round, bool gameOn)
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
