using UnityEngine;
using Zenject;
using GameEventBus;
using GameEventBus.Interfaces;

using SMG.Coordination.Pools;

using SMG.Santas.Scoring;
using SMG.Santas.GameManagement;
using SMG.Santas.RoundFlow;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Coordination
{
  /// <summary>
  /// This is the Zenject Dependency injection mapping.  It also contains a centralized
  /// place for settings to be configured.
  /// </summary>
  public class CatchSceneInstaller : MonoInstaller<CatchSceneInstaller>
  {
    [SerializeField, Tooltip("Settings passed to the CatchMeScript memory pools.")]
    private MemoryPoolSettings CatchMePoolSettings;

    [SerializeField, Tooltip("Settings passed to the Round Manager.")]
    private RoundManager.Settings RoundManagerSettings;

    [SerializeField, Tooltip("Control settings for the 2 player partner game type.")]
    private PartnerControlSet.Settings PartnerControlSetSettings;

    [SerializeField, Tooltip("Control settings for the solo play game type.")]
    private SoloControlSet.Settings SoloControlSetSettings;


    public override void InstallBindings()
    {
      // PresentLists are required for each active Collector, and should not be
      // shared across the collectors
      Container.Bind<PresentList>().AsTransient();

      // This is the main event bus.  It is required by all managers, and many
      // control oriented GameObjects.
      Container.BindInterfacesAndSelfTo<EventBus<IEvent>>().AsSingle();

      // These are overall game state managers.  They generally defer thier
      // implementation to event listeners or supplied strategies.
      Container.Bind<GameManager>().AsSingle();
      Container.Bind<RoundManager>().AsSingle().WithArguments(RoundManagerSettings);
      Container.Bind<ScoreKeeper>().AsSingle();

      // Both the CatchMeFactory and the MemoryPoolGroup are required for the
      // spawning and management of catchable objects.
      Container.BindInterfacesAndSelfTo<CatchMeFactory>().AsTransient();
      Container.BindInterfacesAndSelfTo<MemoryPoolGroup<GameObject, CatchMeScript, CatchMeFactory, CatchMeScript.Pool>>()
        .AsSingle()
        .WithArguments(CatchMePoolSettings);

      BindStrategyLists();
      BindSceneObjects();
    }

    protected void BindStrategyLists()
    {
      // Round Inspectors keep watch on the current round state and control the
      // signalling for end of round.  They are provided to the RoundManager.
      Container.Bind<IRoundInspector>().To<BinCountInspector>().AsSingle();
      // Container.Bind<IRoundInspector>().To<TimeLimitInspector>().AsSingle();
      // Container.Bind<IRoundInspector>().To<TimeCrushInspector>().AsSingle();

      // Scoring Strategies are responsible for checking the correctness of
      // a supplied Collector, then generating a score from that based on the
      // Collector's PresentList and current contents.  They are supplied to
      // the Round Manager.
      Container.Bind<IScoringStrategy>().To<StandardScoring>().AsSingle();
      // Container.Bind<IScoringStrategy>().To<TimeCrushScoring>().AsSingle();

      // ControlSets are resonsible for triggering the spawning of Catchable
      // objects.  They are provided to the Round Manager.
      Container.Bind<IControlSet>().To<PartnerControlSet>().AsSingle().WithArguments(PartnerControlSetSettings);
      Container.BindInterfacesAndSelfTo<SoloControlSet>().AsSingle().WithArguments(SoloControlSetSettings);

    }

    protected void BindSceneObjects()
    {
      // The Scoreboard Display is needed by some Round Inspectors, and is
      // used to update the DetailText and DetailValue fields.
      Container.Bind<ScoreboardDisplay>().FromComponentInHierarchy();
    }
  }
}
