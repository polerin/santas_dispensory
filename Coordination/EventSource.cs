using System.Collections;
using System.Collections.Generic;
using Scoring;
using UnityEngine;
using UnityEngine.Events;

namespace SMG.Coordination {
  /**
   * Central Point for messaging.
   * @TODO Add credit for basis
   * @type {Dictionary}
   */
  public class EventSource {

    private Dictionary<string, UnityEvent> eventDictionary;

    public EventSource() {
      eventDictionary = new Dictionary<string, UnityEvent>();
    }

    public void StartListening(string eventName, UnityAction listener) {
      UnityEvent thisEvent = null;

      if (this.eventDictionary.TryGetValue(eventName, out thisEvent)) {
        thisEvent.AddListener(listener);
      } else {
        thisEvent = new UnityEvent();
        thisEvent.AddListener(listener);
        this.eventDictionary.Add(eventName, thisEvent);
      }
    }

    public void StopListening(string eventName, UnityAction listener) {
      UnityEvent thisEvent = null;
      if (this.eventDictionary.TryGetValue(eventName, out thisEvent)) {
          thisEvent.RemoveListener(listener);
      }
    }


    /**
     * Overloading these because the UnityEvent itself is overloaded.
     */

    public void TriggerEvent(string eventName) {
      UnityEvent thisEvent = null;
      if (this.eventDictionary.TryGetValue(eventName, out thisEvent)) {
        Debug.Log("Triggering: " + eventName + " with no details" );
        thisEvent.Invoke();
      }
    }
  }
}
