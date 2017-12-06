using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using SMG.Santas.Scoring;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.ObjectScripts {
	public class Collector : MonoBehaviour {
		[SerializeField] private GameObject ScreenTextObject;
		[SerializeField] private GameObject ScoreTrigger;
		// public GameObject ScreenTextObj;
		private BinScreen Screen;


		// initial state is "off" (StateOff)
		private int binState = 0;

		public readonly int StateReady = 1;
		public readonly int StateAway = 2;
		public readonly int StateWaiting = 4;

		private RoundManager _RoundManager;

		// the current list of items desired
		private PresentList currentList;

		// Known GameObjects that are currently in the collider
		private List<CatchMeScript> contents = new List<CatchMeScript>();

		void Start() {
			Screen = ScreenTextObject.GetComponent<BinScreen>();
			// BinLever = BinLeverObject.GetComponent<NVRLever>();

		}

		[Inject]
		void Init(RoundManager Manager, PresentList List)
		{
			this._RoundManager = Manager;
			this._RoundManager.Register(this);

			this.AddPresentList(List);

			this.ScoreTrigger.GetComponent<ScoreTrigger>().OnTriggerScoring += this.RedeemList;
		}

		public void AddPresentList(PresentList newList) {
			currentList = newList;

			if (!this.State(StateReady)) {
				this.FlipState(StateReady);
			}
		}

	  public PresentList GetPresentList() {
			return currentList;
		}

		public bool Wait() {
			return this.Wait(true);
		}

		public bool Wait(bool desiredState) {
			if (this.State(this.StateWaiting) != desiredState) {
				this.FlipState(this.StateWaiting);
			}

			return true;
		}

		public void Reset() {
			this.Wait(false);
			this.GetPresentList().GenerateRandomCounts();
			this.EmptyCollector();
		}

		public void Deactivate() {
			this.Wait(true);
			this.EmptyCollector();
		}

		public void EmptyCollector() {
			foreach (CatchMeScript subject in contents) {
				subject.GoAway();
			}
			contents.Clear();
			Screen.UpdateText(this);
		}

		public bool State(int stateValue) {
			return (binState & stateValue) == stateValue;
		}

		void OnTriggerEnter(Collider otherCollider) {
			if (!this.CanAccept()) {
				return;
			}

			if (otherCollider.CompareTag("CatchObject")) {
				this.MarkAdded(otherCollider.gameObject);
			}
		}

		void OnTriggerExit(Collider otherCollider) {
			if (otherCollider.CompareTag("CatchObject")) {
				this.MarkRemoved(otherCollider.gameObject);
			}
		}

		void MarkAdded(GameObject present) {
				CatchMeScript otherScript = present.GetComponent<CatchMeScript>();
				if (otherScript == null) {
					return;
				}

				contents.Add(otherScript);
				otherScript.MarkCollectedBy(this);
		}

		void MarkRemoved(GameObject present) {
				CatchMeScript otherScript = present.GetComponent<CatchMeScript>();

				if (otherScript == null) {
					return;
				}

				contents.Remove(otherScript);
				otherScript.MarkNotCollectedBy(this);
		}

		void RedeemList() {
			if (!this.CanAccept()) {
				return;
			}

			this._RoundManager.ScoreBin(this);
		}

	  void PlaySuccessSound() {
	    AudioSource success = this.GetComponent<AudioSource>();
	    if (success != null) {
	      success.Play();
	    }
	  }

		bool FlipState(int stateValue) {
			binState ^= stateValue;

			return (binState & stateValue) == stateValue;
		}

		bool CanAccept() {
			return (this.State(StateReady) && !this.State(StateAway) && !this.State(StateWaiting));
		}


		public Dictionary<string, int> GetContentCount() {
			Dictionary<string, int> counts = new Dictionary<string, int>();

			foreach (CatchMeScript subject in this.contents) {
				if (!counts.ContainsKey(subject.catchType)) {
					counts[subject.catchType] = 1;
				} else {
					counts[subject.catchType] = (int)counts[subject.catchType] + 1;
				}
			}

			return counts;
		}
	}
}
