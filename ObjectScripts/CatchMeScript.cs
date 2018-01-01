using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SMG.Santas.ObjectScripts {
  public class CatchMeScript : MonoBehaviour {
    // @TODO make this an enum
    public string catchType;

    public float objectLife = 5;
    private bool isCollected = false;

    private IEnumerator destruction;
    private Rigidbody projectile;
    private Pool _Pool;

    public void MarkForDestroy() {
      destruction = DestructionCheck();
      StartCoroutine(destruction);
    }

    protected void Start() {
    }

    protected void SetPool(Pool Pool) {
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

    public void GoAway(bool showAnimation = false)
    {
      if (_Pool == null) {
        Destroy(this);
      } else {
        _Pool.Despawn(this);
      }
    }

    void MarkStayAlive()
    {
      StopCoroutine(destruction);
    }


    IEnumerator DestructionCheck()
    {
      yield return new WaitForSecondsRealtime(objectLife);
      if (isCollected == false) {
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
