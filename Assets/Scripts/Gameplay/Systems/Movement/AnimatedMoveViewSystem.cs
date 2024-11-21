using System.Collections.Generic;
using Gameplay.Components.Animation;
using Gameplay.Components.Chips;
using Gameplay.Components.Chips.Markers;
using Gameplay.Components.Movement;
using Gameplay.Services.AnimationService;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Movement
{
  public class AnimatedMoveViewSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsFilter _viewsNeedsUpdatePositionFilter;

    private EcsPool<MoveViewCommand> _moveViewsCommandsPool;
    private EcsPool<ChipViewRefComponent> _viewsRefsPool;
    private EcsPool<AnimationBufferComponent> _animationBufferPool;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();

      _viewsNeedsUpdatePositionFilter = world
        .Filter<ChipViewRefComponent>()
        .Inc<MoveViewCommand>()
        .Exc<ChipForDestroyComponent>()
        .End();

      _moveViewsCommandsPool = world.GetPool<MoveViewCommand>();
      _viewsRefsPool = world.GetPool<ChipViewRefComponent>();

      _animationBufferPool = world.GetPool<AnimationBufferComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if (_viewsNeedsUpdatePositionFilter.GetEntitiesCount() <= 0)
        return;
      
      List<AnimationCommand> animationCommands = new List<AnimationCommand>();
      
      foreach (int viewEntityIndex in _viewsNeedsUpdatePositionFilter)
      {
        ModifyViewPosition(animationCommands, viewEntityIndex);
        _moveViewsCommandsPool.Del(viewEntityIndex);
      }

      _animationBufferPool.GetRawDenseItems()[1].Buffer.Enqueue(animationCommands);
    }

    private void ModifyViewPosition(List<AnimationCommand> animationCommands, int viewEntityIndex)
    {
      ref ChipViewRefComponent chipViewRefComponent = ref _viewsRefsPool.Get(viewEntityIndex);
      ref MoveViewCommand moveViewCommand = ref _moveViewsCommandsPool.Get(viewEntityIndex);

      animationCommands.Add(new AnimationCommand
      {
        Type = AnimationType.Move,
        TargetObject = chipViewRefComponent.ChipView,
        TargetPosition = moveViewCommand.NewViewPosition
      });

      DeleteMoveCommand();
      
      void DeleteMoveCommand() =>
        _moveViewsCommandsPool.Del(viewEntityIndex);
    }
  }
}