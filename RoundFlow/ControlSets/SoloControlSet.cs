using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using GameEventBus.Interfaces;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.RoundFlow
{
  public class SoloControlSet : AbstractControlSet, IDisposable
  {
    /// <summary>
    /// Injected settings, set in the Installer via the Unity Inspector.
    /// </summary>
    private Settings _Settings;

    /// <summary>
    /// Random source for DispenseRandom()
    /// </summary>
    private System.Random Random;

    /// <summary>
    /// List of Catchable types, used for DispenseRandom()
    /// </summary>
    private Array ItemTypes;

    /// <summary>
    /// This token source is used to safely cancel the await.
    /// Not bothering to inject this one, it feels silly.
    /// </summary>
    CancellationTokenSource TokenSource;

    public SoloControlSet(
      IEventBus<IEvent> EventBus,
      Settings Settings)
      : base(EventBus)
    {
      _Settings = Settings;
      TokenSource = new CancellationTokenSource();
      Random = new System.Random();
      ItemTypes = Enum.GetValues(typeof(CatchTypes));
    }
    
    public void Dispose()
    {
      DestroyTokenSource();
    }

    public override string Slug()
    {
      return "soloControlSet";
    }

    public override void Activate()
    {
      if (isActive == true) {
        Debug.LogWarning("Attempting to activate an already active ControlSet");
        return;
      }

      isActive = true;
      StartDispensing(_Settings.dispenseDelay, TokenSource.Token);
    }

    public override void Deactivate()
    {
      isActive = false;
      TokenSource.Cancel();
    }

    /// <summary>
    /// This method await loops until canceled, dispensing a random Catchable. 
    /// </summary>
    /// <param name="dispenseDelay"></param>
    protected async void StartDispensing(int dispenseDelay, CancellationToken CancelMe)
    {
      try {
        while (!CancelMe.IsCancellationRequested) {
          DispenseRandom();
          await Task.Delay(dispenseDelay, CancelMe);
        }
      } catch (AggregateException ae) {
        //ignoring, we know it'll be canceled at some point.
        Debug.Log("SoloControlSet dispense stopped.");
      }
    }

    protected void DispenseRandom()
    {
      DispenseItem((CatchTypes)ItemTypes.GetValue(Random.Next(ItemTypes.Length))); 
    }

    protected void DestroyTokenSource()
    {
      if (TokenSource == null) {
        return;
      }
      Debug.Log("Destroying Token Source");
      TokenSource.Cancel();
      TokenSource.Dispose();

      // making sure we have actually stopped the generation
      Task.Delay(150).Wait();
      Debug.Log("TokenSourceDestroyed");
      TokenSource = null;
    }

    [Serializable]
    public class Settings
    {
      [Tooltip("The millisecond delay between dispense events.")]
      public int dispenseDelay = 600;
    }
  }
}

