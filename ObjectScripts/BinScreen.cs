using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SMG.Santas.Scoring;


namespace SMG.Santas.ObjectScripts {
	public class BinScreen : MonoBehaviour {
		TextMesh display;

		public string offText = "Bin Off!";
		public string awayText = "Bin Away!";
		public string waitingText = "Bin Waiting!";


		// Use this for initialization
		void Start () {
			display = this.gameObject.GetComponent<TextMesh>();
		}

		public void UpdateText(Collector bin) {

			if (!bin.State(bin.StateReady)) {
				display.text = offText;

			} else if (bin.State(bin.StateAway)){
				display.text = awayText;

			} else if (bin.State(bin.StateWaiting)) {
				display.text = waitingText;

			} else {
				display.text = this.FormatList(bin.GetPresentList());
			}
		}

		string FormatList(PresentList binList) {
				Dictionary<string, int> counts = binList.Counts();
				string result = "";

				foreach (KeyValuePair<string, int> requirement in counts) {
					result += requirement.Key + "\t" + requirement.Value + "\n";
				}

				return result.Trim();
		}
	}
}
