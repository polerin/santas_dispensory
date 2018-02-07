using UnityEngine;
using UnityEngine.UI;
using Zenject;

using SMG.Coordination;
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

    [SerializeField, Tooltip("The Canvas displayed while the game is active.")]
    Canvas DetailCanvas;

    [SerializeField, Tooltip("The Score value text object.")]
    Text ScoreValue;

    [SerializeField, Tooltip("The Round value display.")]
    Text RoundValue;

    [SerializeField, Tooltip("The Errors value text object.")]
    Text ErrorsValue;

    [SerializeField, Tooltip("The per-RoundInspector criteria label.")]
    Text DetailText;
    [SerializeField, Tooltip("The per-RoundInspector value display.")]
    Text DetailValue;

    /// <summary>
    /// Injected Event bus
    /// </summary>
    EventSource _EventSource;

    void Start()
    {
      DisplayFullText(defaultFullText);
    }

    [Inject]
    private void Init(EventSource EventSource)
    {
      _EventSource = EventSource;

      _EventSource.StartListening(GameManager.EVENT_GAMESTART_AFTER, DisplayDetails);
    }

    private void OnDestroy()
    {
      if (_EventSource == null) {
        return;
      }

      _EventSource.StopListening(GameManager.EVENT_GAMESTART_AFTER, DisplayDetails);  
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
  }
}
