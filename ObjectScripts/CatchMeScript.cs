using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchMeScript : MonoBehaviour {
  public string catchType;
  public float objectLife = 40;
  private bool isCollected = false;

  private IEnumerator destruction;

  void Start() {
    // tell the object to wait for a bit then check to destroy itself
    MarkForDestroy();
  }

  public void MarkCollectedBy(Collector Container) {
    isCollected = true;
    MarkStayAlive();
  }

  public void MarkNotCollectedBy(Collector Container) {
    isCollected = false;
    MarkForDestroy();
  }

  public void GoAway(bool showAnimation = false) {
    Destroy(this.gameObject);
  }

  void MarkStayAlive() {
    StopCoroutine(destruction);
  }

  void MarkForDestroy() {
    destruction = DestructionCheck();
    StartCoroutine(destruction);
  }

  IEnumerator DestructionCheck() {
    yield return new WaitForSeconds(objectLife);
    if (isCollected == false) {
      GoAway(true);
    }
  }
}
