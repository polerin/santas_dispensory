using System.Collections;
using UnityEngine;
using Zenject;
using GameEventBus.Interfaces;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;

using SMG.Santas.GameManagement;
using SMG.Santas.Coordination.Events;

namespace SMG.Santas.ObjectScripts
{
  /// <summary>
  /// This is the MonoBehavior that sends a "Start the game!" signal.
  /// It should be attached to a lever object of some sort.
  /// </summary>
  public class StartLever : VRTK_ArtificialRotator
  {
    [SerializeField, Tooltip("How many seconds should the hide animation should last."), Range(0.3f, 3f)]
    private float hideDuration = 0.8f;

    [SerializeField, Tooltip("How small should the lever become as it shrinks."), Range(0.01f, 1f)]
    private float hiddenScale = 0.2f;

    [SerializeField, Tooltip("The curve that the animation follows between initial scale and the hidden scale?")]
    private AnimationCurve hideCurve;

    [SerializeField, Tooltip("The root object. If not set it will default to the parent of this script")]
    private GameObject parentObj;

    [SerializeField, Tooltip("The type of game this lever will start.")]
    private GameTypes GameType;

    /// <summary>
    /// The event bus.  Used to listen for game start/end, and send off "Hey start now!" 
    /// </summary>
    private IEventBus<IEvent> _EventBus;

    /// <summary>
    /// The size the lever should be when active.
    /// </summary>
    private Vector3 InitialSize;

    /// <summary>
    /// The size of the lever at the end of the hide animation
    /// </summary>
    private Vector3 HiddenSize;

    /// <summary>
    /// All GameObjectRenderers 
    /// </summary>
    private MeshRenderer[] AllRenderers;

    /// <summary>
    /// Start time for the hiding animation
    /// </summary>
    private float startTime;

    /// <summary>
    /// Handling setup stuff here, focusing Init for injection.
    /// </summary>
    public void Start ()
    {

      // When the lever hits the end of its travel, the MaxLimitReached event is fired
      MaxLimitReached += LeverPulled;

      if (parentObj == null) {
        parentObj = transform.parent.gameObject;
      }

      //Set up the scaling info for hiding and showing the lever
      InitialSize = parentObj.transform.localScale;
      HiddenSize = InitialSize * hiddenScale;

      AllRenderers = parentObj.GetComponentsInChildren<MeshRenderer>();
    }

    [Inject]
    public void Init(IEventBus<IEvent> EventBus)
    {
      _EventBus = EventBus;
      _EventBus.Subscribe<GameEndAfterEvent>(ShowLever);
    }


    /// <summary>
    /// Callback triggered when the lever hits it's max rotation.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="Event"></param>
    protected void LeverPulled(object Sender, ControllableEventArgs Event)
    {
      StartCoroutine(StartHideCoroutine(hideDuration, hideCurve));

      // TODO Fire off Particles?
      _EventBus.Publish(new StartSignalEvent(GameType));
    }

    /// <summary>
    /// Animator for the hide lever animation
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="Curve"></param>
    /// <returns></returns>
    protected IEnumerator StartHideCoroutine(float duration, AnimationCurve Curve)
    {
      float progress = 0f;

      while (progress < hideDuration) {
        parentObj.transform.localScale = Vector3.LerpUnclamped(InitialSize, HiddenSize, Curve.Evaluate(progress));
        progress += Time.deltaTime;

        yield return null;
      }

      HideLever();
    }

    /// <summary>
    /// This actually hides the lever completely, not just animates the hide
    /// </summary>
    protected void HideLever()
    {
      SetRenderState(false);
    }

    /// <summary>
    /// Resets the lever to initial state
    /// </summary>
    protected void ShowLever(IEvent Event)
    {
      parentObj.transform.localScale = InitialSize;
      ForceRestingPosition();

      // TODO Fire off Particles?
      SetRenderState(true);
    }

    /// <summary>
    /// Loops through all the mesh renderers on the lever and turns them on/off
    /// </summary>
    /// <param name="isActive"></param>
    protected void SetRenderState(bool isActive)
    {
      for (int i = 0; i < AllRenderers.Length; i++) {
        AllRenderers[i].enabled = isActive;
      }
    }

  }
}
