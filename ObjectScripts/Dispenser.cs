using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

using SMG.Coordination;
using SMG.Coordination.Pools;
using SMG.Santas.RoundFlow;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.ObjectScripts
{
  public class Dispenser : MonoBehaviour
  {
    public const string DISPENSE_BEAR = "dispenser_send_bear";
    public const string DISPENSE_BALL = "dispenser_send_ball";
    public const string DISPENSE_PRESENT = "dispenser_send_present";
    public const string DISPENSE_HORSE = "dispenser_send_horse";

    public float shootSpeed = 8.5f;
    public float shootWindow = .5f;

    public float aimAbove = 7f;
    public float aimAboveWindow = .5f;

    public float aimSide = 0;
    public float aimSideWindow = .5f;

    // @TODO inject these
    [SerializeField] ParticleSystem Particles;
    [SerializeField] GameObject PlayerObj;

    bool dispenserActive = false;

    private RoundManager _RoundManager;
    private MemoryPoolGroup<GameObject, CatchMeScript, CatchMeFactory, CatchMeScript.Pool> _ItemPools;


    [Inject]
    void Init(
      RoundManager Manager,
      MemoryPoolGroup<GameObject, CatchMeScript, CatchMeFactory, CatchMeScript.Pool> ItemPools)
    {
      _RoundManager = Manager;
      _RoundManager.Register(this);
      _ItemPools = ItemPools;
    }

    void OnDestroy()
    {
      // _RoundManager.Unregister(this);    //  Should we do something like this?
    }

    public void Activate()
    {
      dispenserActive = true;
    }

    public void Deactivate()
    {
      dispenserActive = false;
    }

    // public interface, negation logic and filtering should be here
    public void DispenseItem(GameObject Prefab)
    {
      if (dispenserActive) {
        SpawnItem(Prefab);
      }
    }

    // spawns an item regardless of state
    void SpawnItem(GameObject Prefab)
    {
      Particles.Play();
      CatchMeScript Item = _ItemPools.Spawn(Prefab);

      Vector3 target = PlayerObj.transform.position - gameObject.transform.position;
      target += getTargetVariance();

      //figure out speed
      Vector3 velocity = (target).normalized * Random.Range(shootSpeed - shootWindow, shootSpeed + shootWindow);

      // Set initial position and speed.
      Item.SetInitial(gameObject.transform.position, velocity);
      Item.MarkForDestroy();
    }

    Vector3 getTargetVariance()
    {
      return new Vector3(
        Random.Range(aimSide - aimSideWindow, aimSide + aimSideWindow),
        Random.Range(aimAbove - aimAboveWindow, aimAbove + aimAboveWindow),
        0
      );
    }

  }
}
