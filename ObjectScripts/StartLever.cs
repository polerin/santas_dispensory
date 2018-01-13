using System;
using System.Collections;
using UnityEngine;
using Zenject;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;

using SMG.Coordination;
using SMG.Santas.GameManagement;

namespace SMG.Santas.ObjectScripts
{
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

    /// <summary>
    /// The event bus.  Used to listen for game start/end, and send off "Hey start now!" 
    /// </summary>
    private EventSource _EventSource;

    private Vector3 ContainerScale;
    private Vector3 InitialSize;
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
      ContainerScale = parentObj.transform.localScale;

      InitialSize = ContainerScale;
      HiddenSize = InitialSize * hiddenScale;

      AllRenderers = parentObj.GetComponentsInChildren<MeshRenderer>();
    }

    [Inject]
    public void Init(EventSource EventSource)
    {
      _EventSource = EventSource;
      _EventSource.StartListening(GameManager.EVENT_GAMEEND_AFTER, ShowLever);
    }

    protected void LeverPulled(object Sender, ControllableEventArgs Event)
    {
      StartCoroutine(StartHideCoroutine(hideDuration, hideCurve));

      // TODO Fire off Particles?
      _EventSource.TriggerEvent(GameManager.EVENT_GAMESTART);
    }

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

    protected void StepScale(float elapsed)
    {
      Debug.Log("Stepping scale: " + elapsed);
    }

    protected void HideLever()
    {
      Debug.Log("Hiding Lever");
      SetRenderState(false);
    }

    protected void ShowLever()
    {
      ContainerScale = InitialSize;
      ForceRestingPosition();

      // TODO Fire off Particles?
      SetRenderState(true);
    }

    protected void SetRenderState(bool isActive)
    {
      for (int i = 0; i < AllRenderers.Length; i++) {
        Debug.Log("Disabling render index: " + i);
        AllRenderers[i].enabled = isActive;
      }
    }

  }
}
