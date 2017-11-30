using System.Collections.Concurrent;

namespace SMG.Coordination.Pools {
  public class SimplePool<T> {
    protected ConcurrentBag<T> _CurrentItems;
    protected Func<T> _Generator;

    public SimplePool<T>(Func<T> Generator) {
      _Generator = Generator;
      _CurrentItems = new ConcurrentBag<T>();
    }

    public void PutObject(T Item) {
      _CurrentItems.Add(Item);
    }

    public T GetObject() {
      T Item;
      if (_CurrentItems.TryTake(out Item)) {
          return Item;
        }
      return _Generator();
    }
  }
}
