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
  public sealed class VerticalShiftSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _chipsWithPositionsFilter;
    private EcsFilter _notBusyCellsFilter;

    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<BusyCellComponent> _busyCellsPool;
    private EcsPool<PlacedChipComponent> _placedChipsPool;
    private EcsPool<FieldComponent> _fieldPool;

    private FieldComponent _field;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsWithPositionsFilter = _world.Filter<ChipComponent>().Inc<GridPositionComponent>().End();
      _notBusyCellsFilter = _world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<BusyCellComponent>().End();

      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();
      _placedChipsPool = _world.GetPool<PlacedChipComponent>();
      _fieldPool = _world.GetPool<FieldComponent>();

      _field = _fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      if(AllCellsAreBusy())
        return;
      
      foreach (int cellEntityIndex in _notBusyCellsFilter)
      {     
        ref GridPositionComponent cellPosition = ref _gridPositionsPool.Get(cellEntityIndex);

        for (int y = cellPosition.Position.y; y < _field.Grid.GetLength(1); y++)
        {
          FreeChipIfItsFreeCellBelow(cellPosition);   
          FreeCell(_field.Grid[cellPosition.Position.x, y].EntityIndex);
        }
      }

      bool AllCellsAreBusy() => 
        _notBusyCellsFilter.GetEntitiesCount() == 0;
    }

    private void FreeChipIfItsFreeCellBelow(GridPositionComponent cellPosition)
    {
      foreach (int chipEntityIndex in _chipsWithPositionsFilter)
      {
        Vector2Int chipPosition = _gridPositionsPool.Get(chipEntityIndex).Position;

        if (chipPosition.x == cellPosition.Position.x && chipPosition.y > cellPosition.Position.y && _placedChipsPool.Has(chipEntityIndex))
        {     
          _placedChipsPool.Del(chipEntityIndex);
          break;
        }
      }
    }

    private void FreeCell(int cellEntityIndex)
    {
      if (_busyCellsPool.Has(cellEntityIndex)) 
        _busyCellsPool.Del(cellEntityIndex);
    }
  }
}