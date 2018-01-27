using System.Collections.Generic;
using UnityEngine.Events;

namespace SMG.Coordination
{
  /// <summary>
  /// Basic implementation of an Event bus.  This is based on the unity tutorials
  /// and could probably be greatly improved.  It definitely needs to support event
  /// details, but I had been stuck on it for far too long.
  /// </summary>
  public class EventSource
  {
    /// <summary>
    /// This is the dictionary of known events.  If a listen call is 
    /// supplied for an unknown event, an event will be created and added
    /// </summary>
    private Dictionary<string, object> eventDictionary;

    public EventSource()
    {
      eventDictionary = new Dictionary<string, object>();
    }

    public void StartListening(string eventName, UnityAction listener)
    {
      UnityEvent thisEvent = null;

      if (!GetEvent(eventName, out thisEvent)) {
        thisEvent = new UnityEvent();
        this.eventDictionary.Add(eventName, thisEvent);
      }

      thisEvent.AddListener(listener);
    }

    public void StopListening(string eventName, UnityAction listener)
    {
      UnityEvent thisEvent = null;

      if (GetEvent(eventName, out thisEvent)) {
        thisEvent.RemoveListener(listener);
      }
    }

    public void TriggerEvent(string eventName)
    {
      UnityEvent thisEvent = null;

      if (GetEvent(eventName, out thisEvent)) {
        thisEvent.Invoke();
      }
    }

    private bool GetEvent<EventClass>(string eventName, out EventClass FoundEvent) where EventClass : UnityEventBase
    {
      if (eventDictionary.ContainsKey(eventName)) {
        FoundEvent = (EventClass)eventDictionary[eventName];
        return true;
      }

      FoundEvent = null;
      return false;
    }
  }
}
