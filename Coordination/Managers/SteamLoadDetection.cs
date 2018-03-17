using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Diagnostics;

public class SteamLoadDetection : MonoBehaviour
{

  bool isLoading = false;

	void Start () {
      UnityEngine.Debug.Log("Loading from start");
      LoadCatchScene();
	}

  protected  void LoadCatchScene()
  {
    Task.Delay(30000).Wait();
    UnityEngine.Debug.Log("Load called");
    Debugger.Break();
    SceneManager.LoadScene("Catch Scene", LoadSceneMode.Additive);
  }

}
