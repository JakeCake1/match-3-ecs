using Components.Cell;
using Components.Chips;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public class PositioningForChipSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _filterChips;
    
    private EcsPool<Chip> _chipPool;
    private EcsPool<GridPosition> _positionsPool;
    private EcsPool<BusyCell> _busyCellPool;
    private EcsPool<PlacedChip> _placedChipPool;
    private EcsPool<ChipViewRef> _chipViewRefPool;
    private EcsPool<Field> _fieldPool;

    private Field _field;

    public void Init(IEcsSystems systems)
    {  
      _world = systems.GetWorld();

      _filterChips = _world.Filter<Chip>().Inc<GridPosition>().Exc<PlacedChip>().End();

      _chipPool = _world.GetPool<Chip>();
      _positionsPool = _world.GetPool<GridPosition>();
      _busyCellPool = _world.GetPool<BusyCell>();
      _placedChipPool = _world.GetPool<PlacedChip>();
      
      _chipViewRefPool = _world.GetPool<ChipViewRef>();

      _fieldPool = _world.GetPool<Field>();

      _field = _fieldPool.GetRawDenseItems()[1];
      
      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {
      if(_filterChips.GetEntitiesCount() == 0)
        return;
      
      foreach (int entityChipIndex in _filterChips)
      {
        ref GridPosition chipPosition = ref _positionsPool.Get(entityChipIndex);

        for (int y = 0; y < _field.Grid.GetLength(1); y++)
        {
          if(_busyCellPool.Has(_field.Grid[chipPosition.Position.x, y].EntityIndex))
            continue;
          
          chipPosition.Position = _field.Grid[chipPosition.Position.x, y].Position;
          _chipViewRefPool.Get(entityChipIndex).ChipView.SetPosition(chipPosition.Position);
            
          _placedChipPool.Add(entityChipIndex);
          _busyCellPool.Add(_field.Grid[chipPosition.Position.x, y].EntityIndex);

          _chipPool.Get(entityChipIndex).ParentCellEnitiyIndex = _field.Grid[chipPosition.Position.x, y].EntityIndex;
            
          break;
        }
      }
      
      Debug.Log($"Run: {GetType().Name}");
    }
  }
}