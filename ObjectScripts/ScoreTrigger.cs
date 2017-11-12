using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour {

	public delegate void TriggerScoringAction();
	public event TriggerScoringAction OnTriggerScoring;

	void OnTriggerEnter(Collider otherCollider) {
		if (otherCollider.CompareTag("Player")) {
			this.OnTriggerScoring();
		}
	}
}
