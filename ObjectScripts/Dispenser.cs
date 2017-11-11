using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Dispenser : MonoBehaviour {
	public float shootSpeed = 8.5f;
	public float shootWindow = .5f;

	public float aimAbove = 7f;
	public float aimAboveWindow = .5f;

	public float aimSide = 0;
	public float aimSideWindow = .5f;

	private RoundManager _RoundManager;

	[SerializeField] GameObject PlayerObj;
	[SerializeField] GameObject bulletPrefab;

	bool dispenserActive = false;

	void Start()
	{

	}


	[Inject]
	void Init(RoundManager Manager)
	{
		this._RoundManager = Manager;
		this._RoundManager.Register(this);
	}

	public void Activate() {
		dispenserActive = true;
	}

	public void Deactivate() {
		dispenserActive = false;
	}

  // public interface, negation logic and filtering should be here
	public void  DispenseItem() {
		if (dispenserActive) {
			SpawnItem();
		}
	}

	void Update()
	{
	}


	// spawns an item regardless of state
	void SpawnItem()
	{
		//Instantiate/Create Bullet
    GameObject tempObj = Instantiate(bulletPrefab);
		tempObj.transform.position = this.gameObject.transform.position;

    //Get the Rigidbody that is attached to that instantiated bullet
    Rigidbody projectile = tempObj.GetComponent<Rigidbody>();
		Vector3 target = PlayerObj.transform.position - tempObj.transform.position;

		target += getTargetVariance();

    //Shoot the Bullet
		projectile.velocity = (target).normalized * Random.Range(shootSpeed - shootWindow, shootSpeed + shootWindow);

    // Destroy (clone.gameObject, 3);
	}

	Vector3 getTargetVariance() {
		return new Vector3(
			Random.Range(aimSide - aimSideWindow, aimSide + aimSideWindow),
			Random.Range(aimAbove - aimAboveWindow, aimAbove + aimAboveWindow),
			0
		);
	}

}
