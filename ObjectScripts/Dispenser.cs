using UnityEngine;
using Zenject;

using SMG.Coordination.Pools;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.ObjectScripts
{
  public class Dispenser : MonoBehaviour
  {
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
        Debug.Log("InDispensasdfasdf");
      if (dispenserActive) {
        Debug.Log("yep it are here");
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
