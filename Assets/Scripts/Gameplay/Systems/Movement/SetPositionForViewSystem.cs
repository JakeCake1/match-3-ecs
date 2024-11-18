using Gameplay.Components.Chips;
using Gameplay.Components.Chips.Markers;
using Gameplay.Components.Movement;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Movement
{
  public class SetPositionForViewSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsFilter _viewsNeedsUpdatePositionFilter;
    
    private EcsPool<MoveViewCommand> _moveViewsCommandsPool;
    private EcsPool<ChipViewRefComponent> _viewsRefsPool;

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
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int viewEntityIndex in _viewsNeedsUpdatePositionFilter)
      {
        ModifyViewPosition(viewEntityIndex);
        _moveViewsCommandsPool.Del(viewEntityIndex);
      }
    }

    private void ModifyViewPosition(int viewEntityIndex)
    {
      ref ChipViewRefComponent chipViewRefComponent = ref _viewsRefsPool.Get(viewEntityIndex);
      ref MoveViewCommand moveViewCommand = ref _moveViewsCommandsPool.Get(viewEntityIndex);
        
      chipViewRefComponent.ChipView.SetPosition(moveViewCommand.NewViewPosition);
    }
  }
}