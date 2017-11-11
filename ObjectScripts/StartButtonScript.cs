using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StartButtonScript : MonoBehaviour {

	protected GameManager GameManager;

	// Use this for initialization
	void Start () {

	}

	[Inject]
	void Init(GameManager manager) {
		this.GameManager = manager;
	}

	// Update is called once per frame
	void Update () {

	}
}
