using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SMG.Santas.Events;

namespace SMG.Coordination {
  /**
   * Central Point for messaging.
   * @type {Dictionary}
   */
  public class EventSource {

    private Dictionary<string, object> eventDictionary;

    public EventSource() {
      eventDictionary = new Dictionary<string, object>();
    }

    public void AddEvent<EventClass>(EventClass NewEvent) where EventClass : ISluggableEvent {
      eventDictionary.Add(NewEvent.Slug(), NewEvent);
    }

    public void StartListening(string eventName, UnityAction listener) {
      UnityEvent thisEvent = null;

      if (!GetEvent(eventName, out thisEvent)) {
        thisEvent = new UnityEvent();
        this.eventDictionary.Add(eventName, thisEvent);
      }

      thisEvent.AddListener(listener);
    }

    public void StopListening(string eventName, UnityAction listener) {
      UnityEvent thisEvent = null;

      if (GetEvent(eventName, out thisEvent)) {
        thisEvent.RemoveListener(listener);
      }
    }

    public void TriggerEvent(string eventName) {
      UnityEvent thisEvent = null;

      if (GetEvent(eventName, out thisEvent)) {
        thisEvent.Invoke();
      }
    }

    private bool GetEvent<EventClass>(string eventName, out EventClass FoundEvent) where EventClass : UnityEventBase {
      if (eventDictionary.ContainsKey(eventName)) {
        FoundEvent = (EventClass)eventDictionary[eventName];
        return true;
      }

      FoundEvent = null;
      return false;
    }
  }
}
