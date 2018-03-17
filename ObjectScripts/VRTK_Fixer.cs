using VRTK;
using UnityEngine;

namespace SMG.Santas.ObjectScripts
{

  public class VRTK_Fixer : MonoBehaviour
  {

    // Use this for initialization
    void Start()
    {
      VRTK_SDKManager.instance.UnloadSDKSetup();
      VRTK_SDKManager.instance.TryLoadSDKSetupFromList();
      this.enabled = false;
    }
  }
}
