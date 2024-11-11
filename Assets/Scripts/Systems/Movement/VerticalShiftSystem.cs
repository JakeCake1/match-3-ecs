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

    private EcsPool<GridPositionComponent> _positionsPool;
    private EcsPool<BusyCellComponent> _busyCellPool;
    private EcsPool<PlacedChipComponent> _placedChipPool;
    private EcsPool<FieldComponent> _fieldPool;

    private FieldComponent _field;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _filterChips = _world.Filter<ChipComponent>().Inc<GridPositionComponent>().End();
      _filterNotBusyCells = _world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<BusyCellComponent>().End();

      _positionsPool = _world.GetPool<GridPositionComponent>();
      _busyCellPool = _world.GetPool<BusyCellComponent>();
      _placedChipPool = _world.GetPool<PlacedChipComponent>();
      
      _fieldPool = _world.GetPool<FieldComponent>();

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
        
        ref GridPositionComponent cellPosition = ref _positionsPool.Get(cellEntity);
        

        for (int y = cellPosition.Position.y; y < _field.Grid.GetLength(1); y++)
        {
          FreeChipIfItsFreeCellBelow(cellPosition, stringBuilder);   
          int cellEntityIndex = _field.Grid[cellPosition.Position.x, y].EntityIndex;
          
          FreeCell(cellEntityIndex, cellPosition, stringBuilder, y);
        }
        
        Debug.Log($"Run: {GetType().Name}\n{stringBuilder}");
      }
    }

    private void FreeChipIfItsFreeCellBelow(GridPositionComponent cellPosition, StringBuilder stringBuilder)
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

    private void FreeCell(int cellEntityIndex, GridPositionComponent cellPosition, StringBuilder stringBuilder, int y)
    {
      if (_busyCellPool.Has(cellEntityIndex))
      {
        _busyCellPool.Del(cellEntityIndex);
        stringBuilder.AppendLine($"Cell Index: {cellEntityIndex} {_field.Grid[cellPosition.Position.x, y].Position}");
      }
    }
  }
}