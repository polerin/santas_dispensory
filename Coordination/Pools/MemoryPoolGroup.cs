using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

// Sligtly more reusable version of https://groups.google.com/forum/#!topic/zenject/4LqbdssSIYs
namespace SMG.Coordination.Pools
{

  // @TODO rework this to support multi param pools?

  public class MemoryPoolGroup<TKey, TMember, TFactory, TPool>
    where TMember : UnityEngine.Component
    where TFactory : IFactory<TMember>
    where TPool : IMemoryPool<TMember>
  {
    readonly MemoryPoolSettings _Settings;
    readonly DiContainer _Container;

    readonly Dictionary<TKey, TPool> Pools = new Dictionary<TKey, TPool>();
    public MemoryPoolGroup(DiContainer Container, MemoryPoolSettings Settings)
    {
      _Settings = Settings;
      _Container = Container;
    }

    public TMember Spawn(TKey poolKey)
    {
      TPool Pool = GetPool(poolKey);
      return Pool.Spawn();
    }

    public void Despawn(TKey poolKey, TMember Member)
    {
      Pools[poolKey].Despawn(Member);
    }


    protected TPool GetPool(TKey poolKey)
    {
      TPool Pool;

      if (!Pools.TryGetValue(poolKey, out Pool)) {
        Pool = CreatePool(poolKey);
        Pools.Add(poolKey, Pool);
      }

      return Pool;
    }

    protected TPool CreatePool(TKey poolKey)
    {
      // LATER.
      return _Container.Instantiate<TPool>(
        new object[] { _Settings, _Container.Instantiate<TFactory>(new object[] { poolKey, _Container }) }
      );
    }
  }
}
