using System.Collections;
using UnityEngine;
using Zenject;
using VRTK;

namespace SMG.Santas.ObjectScripts
{
  /// <summary>
  /// This is the component class for all Catchable object types.
  /// It is responsible for handling it's own motion, collected and
  /// grabbed states, as well as it's despawn/return to pool.
  /// The contained MonoMemoryPool class is used by the MemoryPoolGroup
  /// to help with instanciation lag.
  /// </summary>
  public class CatchMeScript : VRTK_InteractableObject
  {
    [SerializeField, Tooltip("Which catch type should this object report?")]
    private CatchTypes _CatchType;

    [SerializeField, Tooltip("What is the object lifespan when not held or collected?")]
    private float objectLife = 5;

    /// <summary>Flag used as a safeguard in the destruction check</summary>
    private bool isCollected = false;

    /// <summary>Coroutine for distruction check</summary>
    private IEnumerator destruction;

    /// <summary>Reference to this object's rigidbody, used for spanwning</summary>
    private Rigidbody projectile;

    /// <summary>The Zenject pool this object should be placed back into on despawn</summary>
    private Pool _Pool;

    ~CatchMeScript()
    {
      if (_Pool != null) {
        _Pool = null;
      }
    }


    public CatchTypes CatchType()
    {
      return _CatchType;
    }

    /// <summary>
    /// Move the object back to a safe and hidden position.
    /// </summary>
    protected void Reset()
    {
      Vector3 pos = Vector3.zero;
      pos.z -= 10;

      SetInitial(pos, Vector3.zero);
    }

    /// <summary>
    /// Set the initial position and velocity as an object is dispensed
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    public void SetInitial(Vector3 position, Vector3 velocity)
    {
      if (projectile == null) {
        projectile = gameObject.GetComponent<Rigidbody>();
      }

      transform.position = position;
      projectile.velocity = velocity;
    }
    
    /// <summary>
    /// Called when a VRTK controller grabs the object.
    /// This stops the destruction coroutine.  No manual grabbed mark needed,
    /// VRTK_InteractableObject provides IsGrabbed().  This is a template
    /// method called by InteractableObject
    /// @TODO check if the Hand to hand pass works ok.
    /// </summary>
    /// <param name="GrabbedBy"></param>
    public void Grabbed(GameObject GrabbedBy)
    {
      MarkStayAlive();
    }

    /// <summary>
    /// Called when a VRTK controller releases the object.
    /// This starts the destruction coroutine.  No manual grabbed mark needed,
    /// VRTK_InteractableObject provides IsGrabbed().  This is a template
    /// method called by InteractableObject.
    /// </summary>
    /// <param name="ReleasedBy"></param>
    public void UnGrabbed(GameObject ReleasedBy)
    {
      MarkForDestroy();
    }

    /// <summary>
    /// Called when this object enters a Collector trigger.
    /// Marks this object as collected and stops the destruction coroutine.
    /// </summary>
    /// <param name="Container"></param>
    public void MarkCollectedBy(Collector Container)
    {
      isCollected = true;
      MarkStayAlive();
    }

    /// <summary>
    /// Called when this object leaves a Collector trigger.
    /// Marks this object as no longer collected and starts the destruction countdown.
    /// </summary>
    /// <param name="Container"></param>
    public void MarkNotCollectedBy(Collector Container)
    {
      isCollected = false;
      MarkForDestroy();
    }

    /// <summary>
    /// Stop the coroutine that would have destroyed an object as time expires
    /// </summary>
    void MarkStayAlive()
    {
      StopCoroutine(destruction);
    }

    /// <summary>
    /// Start the Coroutine that will destroy this object as time expires
    /// </summary>
    public void MarkForDestroy()
    {
      destruction = DestructionCheck();
      StartCoroutine(destruction);
    }
    
    /// <summary>
    /// Despawn this object or return it to the pool
    /// </summary>
    /// <param name="showAnimation"></param>
    public void GoAway(bool showAnimation = false)
    {
      if (_Pool == null) {
        Destroy(this);
      } else {
        _Pool.Despawn(this);
      }
    }

    /// <summary>
    /// Grab a reference to the MemoryPool this prefab should return too on despawn
    /// </summary>
    /// <param name="Pool"></param>
    protected void SetPool(Pool Pool)
    {
      _Pool = Pool;
    }

    /// <summary>
    /// Coroutine that attempts to despawn the object when time expires
    /// </summary>
    /// <returns></returns>
    IEnumerator DestructionCheck()
    {
      yield return new WaitForSecondsRealtime(objectLife);
      if (isCollected == false && !IsGrabbed()) {
        GoAway(true);
      }
    }

    /// <summary>
    /// Zenject Memory pool.
    /// An instance of this is injected into the MemoryPoolGroup.
    /// </summary>    
    public class Pool : MonoMemoryPool<CatchMeScript>
    {
      protected override void OnCreated(CatchMeScript Item)
      {
        Item.SetPool(this);
      }
    }
  }
}
