using Zenject;
using UnityEngine;

using SMG.Santas.ObjectScripts;

namespace SMG.Santas.ObjectScripts
{
  public class CatchMeFactory : IFactory<CatchMeScript>
  {
    readonly DiContainer _Container;
    readonly GameObject _Prefab;

    public CatchMeFactory(GameObject Prefab, DiContainer Container)
    {
      _Container = Container;
      _Prefab = Prefab;
    }

    public CatchMeScript Create()
    {
      return _Container.InstantiatePrefabForComponent<CatchMeScript>(_Prefab);
    }
  }
}
