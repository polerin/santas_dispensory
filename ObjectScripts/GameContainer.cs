using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameContainer : MonoBehaviour {

	private EventSource _EventSource;
	private GameManager _GameManager;

	// Use this for initialization
	void Start () {

	}

	[Inject]
	void Init(EventSource EventSource, GameManager GameManager)
	{
		this._GameManager = GameManager;
		this._EventSource = EventSource;
	}

	// Update is called once per frame
	void Update () {
		// @TODO make this not crap

		// @TODO delete me when the Bin score lever is in.
		if (Input.GetKeyDown(KeyCode.P)) {
			this._EventSource.TriggerEvent("score_bin");
		}

		if (Input.GetKeyDown(KeyCode.N)) {
			Debug.Log("Event Game start");
			this._EventSource.TriggerEvent(GameManager.EVENT_GAMESTART);
		}

		if (Input.GetKeyDown(KeyCode.Keypad1)) {
			this._EventSource.TriggerEvent(Dispenser.EVENT_PREFIX + "bear");
		}


		if (Input.GetKeyDown(KeyCode.Keypad2)) {
			this._EventSource.TriggerEvent(Dispenser.EVENT_PREFIX + "present");
		}

		if (Input.GetKeyDown(KeyCode.Keypad3)) {
			this._EventSource.TriggerEvent(Dispenser.EVENT_PREFIX + "ball");
		}

		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			this._EventSource.TriggerEvent(Dispenser.EVENT_PREFIX + "horse");
		}
	}
}
