using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

using SMG.Coordination;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.ObjectScripts {
	public class Dispenser : MonoBehaviour {

		public const string EVENT_PREFIX = "dispenser_send_";

		public float shootSpeed = 8.5f;
		public float shootWindow = .5f;

		public float aimAbove = 7f;
		public float aimAboveWindow = .5f;

		public float aimSide = 0;
		public float aimSideWindow = .5f;

		[SerializeField] ParticleSystem Particles;
		[SerializeField] GameObject PlayerObj;
		[SerializeField] GameObject bulletPrefab;

		bool dispenserActive = false;

		private UnityAction m_DispenseItem;
		private RoundManager _RoundManager;
		private EventSource _EventSource;


		[Inject]
		void Init(RoundManager Manager, EventSource EventSource)
		{
			// Add our DispenseItem() to the unity action
			m_DispenseItem += DispenseItem;

			_RoundManager = Manager;
			_RoundManager.Register(this);

			_EventSource = EventSource;
			_EventSource.StartListening(GetDispenseEventName(), m_DispenseItem);
		}

		void OnDestroy()
		{
			if (_EventSource == null) {
				return;
			}

			_EventSource.StopListening(GetDispenseEventName(), m_DispenseItem);
		}

		protected string GetDispenseEventName() {
			return Dispenser.EVENT_PREFIX + bulletPrefab.GetComponent<CatchMeScript>().catchType;
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

		// spawns an item regardless of state
		void SpawnItem()
		{
			Particles.Play();
			//Instantiate/Create Bullet
	    GameObject tempObj = Instantiate(bulletPrefab);
			tempObj.transform.position = gameObject.transform.position;

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
}
