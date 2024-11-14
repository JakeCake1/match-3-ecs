using Components.Chips;
using Components.Chips.Markers;
using Components.Movement;
using Leopotam.EcsLite;

namespace Systems.Movement
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
        ref ChipViewRefComponent chipViewRefComponent = ref _viewsRefsPool.Get(viewEntityIndex);
        ref MoveViewCommand moveViewCommand = ref _moveViewsCommandsPool.Get(viewEntityIndex);
        
        chipViewRefComponent.ChipView.SetPosition(moveViewCommand.NewViewPosition);
        
        _moveViewsCommandsPool.Del(viewEntityIndex);
      }
    }
  }
}