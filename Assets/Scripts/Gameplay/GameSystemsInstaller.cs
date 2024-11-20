using Gameplay.Systems.Animation;
using Gameplay.Systems.Camera;
using Gameplay.Systems.Chips;
using Gameplay.Systems.Control;
using Gameplay.Systems.Field_State;
using Gameplay.Systems.Grid;
using Gameplay.Systems.Injector;
using Gameplay.Systems.Movement;
using Gameplay.Systems.Score;
using Gameplay.Systems.Score.View_Systems;
using Leopotam.EcsLite;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameSystemsInstaller : IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, CreateFieldInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateCellViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CameraResizeInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateChipsInjectorsInitSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, CreateChipsSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateChipsViewsSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, FindSwapsSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, SwapSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, SetPositionInGridSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, AnimatedMoveViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, RechargeInjectorsSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, DestroyChipsSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, DestroyChipsViewsSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, VerticalShiftSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, VerticalCheckSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HorizontalCheckSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CombineCommandsSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, ExecuteMergeSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, ReturnNotMergedSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, AnimationExecutionSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, ControlSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, ScoreCountSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateScoreViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateAddScoreSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateAddScoreViewSystem>(Lifetime.Scoped);
    }
  }
}