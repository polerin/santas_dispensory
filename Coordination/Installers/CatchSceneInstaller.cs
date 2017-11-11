using UnityEngine;
using Zenject;
using Scoring;

/**
 * This is the zenject catchscene dependency injection mapping
 */
public class CatchSceneInstaller : MonoInstaller<CatchSceneInstaller>
{
    public override void InstallBindings()
    {
      Container.Bind<IScoringStrategy>().To<StandardScoring>().AsCached();

      Container.Bind<GameManager>().AsSingle();
      Container.Bind<RoundManager>().AsSingle();
      Container.Bind<ScoreKeeper>().AsSingle();
    }
}
