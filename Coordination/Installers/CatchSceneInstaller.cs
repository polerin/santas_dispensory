using UnityEngine;
using Zenject;
using SMG.Coordination;
using SMG.Coordination.Pools;

using SMG.Santas.Scoring;
using SMG.Santas.GameManagement;
using SMG.Santas.RoundFlow;
using SMG.Santas.Events;
using SMG.Santas.ObjectScripts;

using UnityEngine.Events;

namespace SMG.Santas.Coordination {
  /**
   * This is the zenject catchscene dependency injection mapping
   */
  public class CatchSceneInstaller : MonoInstaller<CatchSceneInstaller> {
    [SerializeField]
    private MemoryPoolSettings CatchMePoolSettings;
    [SerializeField]
    private RoundManager.Settings RoundManagerSettings;

    public override void InstallBindings() {
      Container.Bind<IScoringStrategy>().To<StandardScoring>().AsCached();

      Container.Bind<PresentList>().AsTransient();

      Container.Bind<GameManager>().AsSingle();
      Container.Bind<RoundManager>().AsSingle().WithArguments(RoundManagerSettings);
      Container.Bind<ScoreKeeper>().AsSingle();
      Container.Bind<EventSource>().AsSingle();

      Container.Bind<IRoundInspector>().To<BinCountInspector>().AsSingle();
      // Container.Bind<IRoundInspector>().To<TimeLimitInspector>().AsSingle();

      Container.BindInterfacesAndSelfTo<CatchMeFactory>().AsTransient();
      Container.BindInterfacesAndSelfTo<MemoryPoolGroup<GameObject, CatchMeScript, CatchMeFactory, CatchMeScript.Pool>>()
        .AsSingle()
        .WithArguments(CatchMePoolSettings);
    }
  }
}
