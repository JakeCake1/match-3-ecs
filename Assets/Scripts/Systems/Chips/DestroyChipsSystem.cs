using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;

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
    private EcsPool<ChipViewForDestroyComponent> _chipsViewForDestroyPool;

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
      _chipsViewForDestroyPool = _world.GetPool<ChipViewForDestroyComponent>();  
      
      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if(NothingToDestroy())
        return;
      
      ref ChipsFieldComponent chipsField = ref _chipsFieldPool.GetRawDenseItems()[1];

      foreach (int chipEntityIndex in _chipsForDestroyFilter)
      {      
        MarkCellAsNotBusy(chipEntityIndex);
        RequestForChipViewDestroy(chipEntityIndex);
        ResetChipInGrid(chipEntityIndex, ref chipsField);
        DestroyChip(chipEntityIndex);
      }

      bool NothingToDestroy() => 
        _chipsForDestroyFilter.GetEntitiesCount() == 0;
    }

    private void MarkCellAsNotBusy(int chipEntityIndex) => 
      _busyCellsPool.Del(_chipsPool.Get(chipEntityIndex).ParentCellEntityIndex);

    private void RequestForChipViewDestroy(int chipEntityIndex)
    {
      int destroyViewCommandEntity = _world.NewEntity();
      ref ChipViewForDestroyComponent chipForDestroyComponent = ref _chipsViewForDestroyPool.Add(destroyViewCommandEntity);
      chipForDestroyComponent.ChipView = _chipViewsPool.Get(chipEntityIndex).ChipView;
    }

    private void ResetChipInGrid(int chipEntityIndex, ref ChipsFieldComponent chipsField)
    {
      ref GridPositionComponent gridPosition = ref _gridPositionsPool.Get(chipEntityIndex);

      int positionX = gridPosition.Position.x;
      int positionY = gridPosition.Position.y;
        
      chipsField.Grid[positionX, positionY] = default;
      chipsField.Grid[positionX, positionY] = -1;
    }

    private void DestroyChip(int chipEntityIndex) => 
      _world.DelEntity(chipEntityIndex);
  }
}