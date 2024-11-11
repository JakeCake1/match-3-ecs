using System.Text;
using Components.Cell;
using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Movement
{
  public class VerticalShiftSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _filterChips;
    private EcsFilter _filterNotBusyCells;

    private EcsPool<GridPosition> _positionsPool;
    private EcsPool<BusyCell> _busyCellPool;
    private EcsPool<PlacedChip> _placedChipPool;
    private EcsPool<Field> _fieldPool;

    private Field _field;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _filterChips = _world.Filter<Chip>().Inc<GridPosition>().End();
      _filterNotBusyCells = _world.Filter<Cell>().Inc<GridPosition>().Exc<BusyCell>().End();

      _positionsPool = _world.GetPool<GridPosition>();
      _busyCellPool = _world.GetPool<BusyCell>();
      _placedChipPool = _world.GetPool<PlacedChip>();
      
      _fieldPool = _world.GetPool<Field>();

      _field = _fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      if(_filterNotBusyCells.GetEntitiesCount() == 0)
        return;
      
      foreach (int cellEntity in _filterNotBusyCells)
      {     
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Free chips at this turn:");
        
        ref GridPosition cellPosition = ref _positionsPool.Get(cellEntity);
        

        for (int y = cellPosition.Position.y; y < _field.Grid.GetLength(1); y++)
        {
          FreeChipIfItsFreeCellBelow(cellPosition, stringBuilder);   
          int cellEntityIndex = _field.Grid[cellPosition.Position.x, y].EntityIndex;
          
          FreeCell(cellEntityIndex, cellPosition, stringBuilder, y);
        }
        
        Debug.Log($"Run: {GetType().Name}\n{stringBuilder}");
      }
    }

    private void FreeChipIfItsFreeCellBelow(GridPosition cellPosition, StringBuilder stringBuilder)
    {
      foreach (int chipEntity in _filterChips)
      {
        Vector2Int chipPosition = _positionsPool.Get(chipEntity).Position;

        if (chipPosition.x == cellPosition.Position.x && chipPosition.y > cellPosition.Position.y && _placedChipPool.Has(chipEntity))
        {     
          stringBuilder.AppendLine($"Chip Index: {chipEntity} {chipPosition}");
          _placedChipPool.Del(chipEntity);
          break;
        }
      }
    }

    private void FreeCell(int cellEntityIndex, GridPosition cellPosition, StringBuilder stringBuilder, int y)
    {
      if (_busyCellPool.Has(cellEntityIndex))
      {
        _busyCellPool.Del(cellEntityIndex);
        stringBuilder.AppendLine($"Cell Index: {cellEntityIndex} {_field.Grid[cellPosition.Position.x, y].Position}");
      }
    }
  }
}