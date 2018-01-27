using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SMG.Santas.Scoring;

namespace SMG.Santas.ObjectScripts
{
  public class BinScreen : MonoBehaviour
  {
    Text display;

    public string offText = "Bin Off!";
    public string awayText = "Bin Away!";
    public string waitingText = "Bin Waiting!";


    // Use this for initialization
    void Start()
    {
      display = this.gameObject.GetComponent<Text>();
    }

    public void UpdateText(Collector bin)
    {

      if (bin.isActive) {
        display.text = this.FormatList(bin.GetPresentList());

      } else { 
        display.text = offText;
      }
    }

    string FormatList(PresentList binList)
    {
      Dictionary<string, int> counts = binList.Counts();
      string result = "";

      foreach (KeyValuePair<string, int> requirement in counts) {
        result += requirement.Key + "\t" + requirement.Value + "\n";
      }

      return result.Trim();
    }
  }
}
