using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Common;
using Leopotam.EcsLite;

namespace Systems.Chips
{
  public sealed class DestroyChipsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipsForDestroyFilter;
    
    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<BusyCellComponent> _busyCellsPool;
    private EcsPool<ChipViewRefComponent> _chipViewsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsForDestroyFilter = _world.Filter<ChipComponent>()
        .Inc<GridPositionComponent>()
        .Inc<ChipViewRefComponent>()
        .Inc<ChipForDestroyComponent>()
        .End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();
      _chipViewsPool = _world.GetPool<ChipViewRefComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if(NothingToDestroy())
        return;
      
      foreach (int chipEntityIndex in _chipsForDestroyFilter)
      {       
        _chipViewsPool.Get(chipEntityIndex).ChipView.Destroy();
        _busyCellsPool.Del(_chipsPool.Get(chipEntityIndex).ParentCellEntityIndex);
        _world.DelEntity(chipEntityIndex);
      }

      bool NothingToDestroy() => 
        _chipsForDestroyFilter.GetEntitiesCount() == 0;
    }
  }
}