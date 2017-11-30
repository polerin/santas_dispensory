using UnityEngine;
using Zenject;
using SMG.Coordination;

using SMG.Santas.Scoring;
using SMG.Santas.GameManagement;
using SMG.Santas.RoundFlow;
using SMG.Santas.RoundFlow.RoundInspectors;

namespace SMG.Santas.Coordination {
  /**
   * This is the zenject catchscene dependency injection mapping
   */
  public class CatchSceneInstaller : MonoInstaller<CatchSceneInstaller> {
    public override void InstallBindings() {
      Container.Bind<IScoringStrategy>().To<StandardScoring>().AsCached();

      Container.Bind<PresentList>().AsTransient();

      Container.Bind<GameManager>().AsSingle();
      Container.Bind<RoundManager>().AsSingle();
      Container.Bind<ScoreKeeper>().AsSingle();
      Container.Bind<EventSource>().AsSingle();

      Container.Bind<IRoundInspector>().To<BinCountInspector>().AsSingle();
      Container.Bind<IRoundInspector>().To<TimeLimitInspector>().AsSingle();
    }
  }
}
