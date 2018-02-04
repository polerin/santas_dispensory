using System.Collections.Generic;
using UnityEngine;
using Zenject;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;

using SMG.Santas.Scoring;
using SMG.Santas.RoundFlow;

namespace SMG.Santas.ObjectScripts
{
  /// <summary>
  /// This is the main bin script.  It is responsible
  /// for mainting bin state, including CatchMe objects
  /// and Present list.
  /// </summary>
  public class Collector : MonoBehaviour
  {
    [SerializeField, Tooltip("When it hits MaxRotation, this rotator signals that the bin should be scored.")]
    private VRTK_ArtificialRotator SendIndicator;

    [SerializeField, Tooltip("This controls the display shown for the bin")]
    private BinScreen Screen;
    
    /// <summary>
    /// initial state is "off"
    /// </summary>
    public bool isActive { get; protected set; } = false;

    /// <summary>
    /// The Roundmanager is responsible for controlling all of the prefabs in the scene
    /// </summary>
    private RoundManager _RoundManager;

    /// <summary>
    /// The Current list of needed items
    /// </summary>
    private PresentList currentList;

    /// <summary>
    /// GameObjects that are currently in the collider
    /// </summary>
    private List<CatchMeScript> contents = new List<CatchMeScript>();

    [Inject]
    void Init(RoundManager Manager, PresentList List)
    {
      this._RoundManager = Manager;
      this._RoundManager.Register(this);

      this.AddPresentList(List); //realstically this probably should not be injected.

      SendIndicator.MaxLimitReached += LidClosed;
    }

    /// <summary>
    /// VRTK event handler, Fired when lid reaches max limit
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="Event"></param>
    private void LidClosed(object Sender, ControllableEventArgs Event)
    {
      if (!isActive) {
        return;
      }

      RedeemList();
      PlayScoreSound();
      // @TODO Play Score particle
      // @TODO Lock Lid for scoring down time?
    }

    /// <summary>
    /// Add a present list to the bin.
    /// By default, this will make the list the Current List
    /// </summary>
    /// <param name="newList"></param>
    public void AddPresentList(PresentList newList)
    {
      currentList = newList;
    }

    /// <summary>
    /// @TODO Switch to getter
    /// </summary>
    /// <returns></returns>
    public PresentList GetPresentList()
    {
      return currentList;
    }

    /// <summary>
    /// @TODO this needs a rename. Look in RoundManager.
    /// </summary>
    public void Reset()
    {
      this.Activate();
      this.GetPresentList().GenerateRandomCounts();
      this.EmptyCollector();
    }
    
    /// <summary>
    /// Mark the bin as ready to be used, open the lid
    /// </summary>
    public void Activate()
    {
      //OpenLid();
      isActive = true;
    }

    /// <summary>
    /// Mark the bin as not ready to be used and close the lid
    /// </summary>
    public void Deactivate()
    {
      //CloseLid();
      isActive = false;
      this.EmptyCollector();
    }

    /// <summary>
    /// Remove all catchme scripts from the collector.
    /// </summary>
    public void EmptyCollector()
    {
      foreach (CatchMeScript subject in contents) {
        subject.GoAway();
      }
      contents.Clear();
      Screen.UpdateText(this);
    }

    /// <summary>
    /// Trigger enter, only used if bin is active.
    /// Marks catch objects as collected.
    /// </summary>
    /// <param name="otherCollider"></param>
    void OnTriggerEnter(Collider otherCollider)
    {
      if (!this.isActive) {
        return;
      }

      if (otherCollider.CompareTag("CatchObject")) {
        this.MarkAdded(otherCollider.gameObject);
      }
    }

    /// <summary>
    /// Trigger exit, Marks catch objects as no longer collected.
    /// </summary>
    /// <param name="otherCollider"></param>
    void OnTriggerExit(Collider otherCollider)
    {
      if (otherCollider.CompareTag("CatchObject")) {
        this.MarkRemoved(otherCollider.gameObject);
      }
    }

    /// <summary>
    /// Mark CatchmeScript objects as collected
    /// </summary>
    /// <param name="present"></param>
    void MarkAdded(GameObject present)
    {
      CatchMeScript otherScript = present.GetComponent<CatchMeScript>();
      if (otherScript == null) {
        return;
      }

      contents.Add(otherScript);
      otherScript.MarkCollectedBy(this);
    }

    /// <summary>
    /// Mark CatchmeScript object as no longer collected
    /// </summary>
    /// <param name="present"></param>
    void MarkRemoved(GameObject present)
    {
      CatchMeScript otherScript = present.GetComponent<CatchMeScript>();

      if (otherScript == null) {
        return;
      }

      contents.Remove(otherScript);
      otherScript.MarkNotCollectedBy(this);
    }

    /// <summary>
    /// Send the bin to the RoundManager to be scored
    /// @TODO make this return a bool for particle usage?
    /// </summary>
    void RedeemList()
    {
      if (!isActive) {
        return;
      }

      _RoundManager.ScoreBin(this);
    }

    /// <summary>
    /// Play a success sound
    /// @TODO stop using GetComponent here
    /// </summary>
    void PlayScoreSound()
    {
      AudioSource success = this.GetComponent<AudioSource>();
      if (success != null) {
        success.Play();
      }
    }

    /// <summary>
    /// Get the per-type count of CatchMeScript objects.
    /// @TODO This needs to be reworked for the CatchTypes Enum.
    /// </summary>
    /// <returns></returns>
    public Dictionary<CatchTypes, int> GetContentCount()
    {
      Dictionary<CatchTypes, int> counts = new Dictionary<CatchTypes, int>();
      CatchTypes CurrentType;

      foreach (CatchMeScript subject in this.contents) {
        CurrentType = subject.CatchType(); 
        if (!counts.ContainsKey(CurrentType)) {
          counts[CurrentType] = 1;
        } else {
          counts[CurrentType] = (int)counts[CurrentType] + 1;
        }
      }

      return counts;
    }
  }
}
