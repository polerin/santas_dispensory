using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using GameEventBus.Interfaces;

using System;

namespace SMG.Santas.RoundFlow
{
  public class SoloControlSet : AbstractControlSet, IDisposable
  {
    /// <summary>
    /// Injected settings, set in the Installer via the Unity Inspector.
    /// </summary>
    private Settings _Settings;

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
    }

    public void Dispose()
    {
      TokenSource.Dispose();
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


    public class Settings
    {
      [Tooltip("The millisecond delay between dispense events.")]
      public int dispenseDelay = 600;
    }
  }
}

