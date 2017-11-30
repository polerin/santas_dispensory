using System.Collections;
using System.Collections.Concurrent;

namespace SMG.Coordination.Pools {

  public class MultiPool<T> {
    // need a Dictionary and methods to select and call the add/remove.
    private Dictionary<string, SimplePool> Pools = new Dictionary<string, SimplePool>();

    // @TODO use reflection to generate the correct default generator for a passed in Put/Get Object? Or just remove
    private Func<T> _defaultGenerator;

    public MultiPool<T> (Func<T> generator) {
      _defaultGenerator = generator;
    }

    public void AddPool(string poolName, Func<T> Generator) {
      Pools.Add(poolName, new SimplePool<T>(Generator));
    }

    public void PutObject(string poolName, T Item) {
      SimplePool<T> TargetPool = null;
      if (!Pools.TryGetValue(poolName, out TargetPool)) {
        TargetPool = new SimplePool<T>();
        TargetPool.PutItem(Item):
        Pools.Add(poolName, TargetPool);
      } else {
        TargetPool.PutObject(Item);
      }
    }

    public T GetObject(string poolName) {
      SimplePool<T> TargetPool = null;

      if (Pools.TryGetValue(poolName, out TargetPool)) {
        return TargetPool.GetObject();
      } else {
        return _defaultGenerator();  // probably not what you are looking for.  Not Great.  throw exception?
      }
    }

  }
}
