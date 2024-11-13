using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public sealed class DestroyChipsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipsForDestroyFilter;

    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<BusyCellComponent> _busyCellsPool;
    private EcsPool<ChipViewRefComponent> _chipViewsPool;
    private EcsPool<ChipsFieldComponent> _chipsFieldPool;
    
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
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();  
      
      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if(true || NothingToDestroy())
        return;
      
      ref ChipsFieldComponent chipsField = ref _chipsFieldPool.GetRawDenseItems()[1];

      foreach (int chipEntityIndex in _chipsForDestroyFilter)
      {      
        ref GridPositionComponent gridPosition = ref _gridPositionsPool.Get(chipEntityIndex);

        int positionX = gridPosition.Position.x;
        int positionY = gridPosition.Position.y;
        
        _chipViewsPool.Get(chipEntityIndex).ChipView.Destroy();
        _busyCellsPool.Del(_chipsPool.Get(chipEntityIndex).ParentCellEntityIndex);
        
        _world.DelEntity(chipEntityIndex);
        
        chipsField.Grid[positionX, positionY] = default;
        chipsField.Grid[positionX, positionY].EntityIndex = -1;
      }

      bool NothingToDestroy() => 
        _chipsForDestroyFilter.GetEntitiesCount() == 0;
    }
  }
}