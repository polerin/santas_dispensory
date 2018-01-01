using UnityEngine.Events;
using SMG.Santas.Events;

namespace SMG.Santas.Events {
  [System.Serializable]
  public class DispenseEvent : UnityEvent<int>, ISluggableEvent {
    public const string SLUG = "DispenseEvent";
    public const int DISPENSE_BEAR = 0;
    public const int DISPENSE_BALL = 1;
    public const int DISPENSE_PRESENT = 2;
    public const int DISPENSE_HORSE = 4;

    public string Slug() {
      return DispenseEvent.SLUG;
    }
  }
}
