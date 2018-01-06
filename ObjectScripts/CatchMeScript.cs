using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using VRTK;

namespace SMG.Santas.ObjectScripts
{
  public class CatchMeScript : MonoBehaviour
  {
    // @TODO make this an enum
    public string catchType;

    public float objectLife = 5;
    private bool isCollected = false;
    private bool isHeld = false;

    private IEnumerator destruction;
    private Rigidbody projectile;
    private Pool _Pool;


    protected void Start()
    {
      gameObject.GetComponet<VRTK_InteractableObject>();
    }

    protected void SetPool(Pool Pool)
    {
      _Pool = Pool;
    }

    protected void Reset()
    {
      Vector3 pos = Vector3.zero;
      pos.z -= 10;

      SetInitial(pos, Vector3.zero);
    }

    public void SetInitial(Vector3 position, Vector3 velocity)
    {
      if (projectile == null) {
        projectile = gameObject.GetComponent<Rigidbody>();
      }

      transform.position = position;
      projectile.velocity = velocity;
    }

    public void MarkHeld()
    {
      isHeld = true;
      MarkStayAlive();
    }

    public void MarkNotHeld()
    {
      isHeld = false;
      MarkForDestroy();
    }

    public void MarkCollectedBy(Collector Container)
    {
      isCollected = true;
      MarkStayAlive();
    }

    public void MarkNotCollectedBy(Collector Container)
    {
      isCollected = false;
      MarkForDestroy();
    }

    void MarkStayAlive()
    {
      StopCoroutine(destruction);
    }

    public void MarkForDestroy()
    {
      destruction = DestructionCheck();
      StartCoroutine(destruction);
    }

    public void GoAway(bool showAnimation = false)
    {
      if (_Pool == null) {
        Destroy(this);
      } else {
        _Pool.Despawn(this);
      }
    }

    IEnumerator DestructionCheck(int )
    {
      yield return new WaitForSecondsRealtime(objectLife);
      if (isCollected == false && isHeld == false) {
        GoAway(true);
      }
    }

    // Zenject factory
    public class Pool : MonoMemoryPool<CatchMeScript>
    {
      protected override void OnCreated(CatchMeScript Item)
      {
        Item.SetPool(this);
      }
    }
  }
}
