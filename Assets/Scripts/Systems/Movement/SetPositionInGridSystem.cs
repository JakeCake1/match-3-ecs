using System;
using System.Collections.Generic;
using System.Linq;
using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Common;
using Components.Field;
using Components.Movement;
using Leopotam.EcsLite;

namespace Systems.Movement
{
  public sealed class SetPositionInGridSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _notPlacedChipsFilter;

    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _positionsPool;
    private EcsPool<BusyCellComponent> _busyCellsPool;
    private EcsPool<PlacedChipComponent> _placedChipsPool;
    private EcsPool<MoveViewCommand> _moveViewsCommandsPool;
    private EcsPool<FieldComponent> _fieldPool;

    private FieldComponent _field;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _notPlacedChipsFilter = _world.Filter<ChipComponent>().Inc<GridPositionComponent>().Exc<PlacedChipComponent>().End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _positionsPool = _world.GetPool<GridPositionComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();
      _placedChipsPool = _world.GetPool<PlacedChipComponent>();
      _moveViewsCommandsPool = _world.GetPool<MoveViewCommand>();

      _fieldPool = _world.GetPool<FieldComponent>();
      _field = _fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      if (AllChipsPlaced())
        return;
      
      var sortedChips = SortFreeChipsEntities(); //TODO Кэшировать фишки по Y позиции

      foreach (var freeChip in sortedChips)
      {
        ref GridPositionComponent chipPosition = ref _positionsPool.Get(freeChip.Key);
        int targetY = FindLowestNotBusyPositionInGrid(chipPosition, out int targetCellEntityIndex);
        MarkChipAndPlace(targetY, freeChip.Key, targetCellEntityIndex);
      }

      bool AllChipsPlaced() => 
        _notPlacedChipsFilter.GetEntitiesCount() == 0;
    }

    private int FindLowestNotBusyPositionInGrid(GridPositionComponent chipPosition, out int targetCellEntityIndex)
    {
      int targetY = 0;
      targetCellEntityIndex = 0;

      int startY = Math.Clamp(chipPosition.Position.y, 0, _field.Grid.GetLength(1) - 1);

      for (int y = startY; y >= 0; y--)
      {
        if (_busyCellsPool.Has(_field.Grid[chipPosition.Position.x, y].EntityIndex))
          break;

        targetCellEntityIndex = _field.Grid[chipPosition.Position.x, y].EntityIndex;
        targetY = y;
      }

      return targetY;
    }

    private void MarkChipAndPlace(int y, int freeChipEntityIndex, int cellEntityIndex)
    {
      ref GridPositionComponent chipPosition = ref _positionsPool.Get(freeChipEntityIndex);
      
      chipPosition.Position = _field.Grid[chipPosition.Position.x, y].Position;

      ref MoveViewCommand moveViewCommand = ref _moveViewsCommandsPool.Add(freeChipEntityIndex);
      moveViewCommand.NewViewPosition = chipPosition.Position;
      
      _placedChipsPool.Add(freeChipEntityIndex);
      
      ref BusyCellComponent busyCellComponent = ref _busyCellsPool.Add(cellEntityIndex);
      
      busyCellComponent.ChipEntityIndex = freeChipEntityIndex;
      
      _chipsPool.Get(freeChipEntityIndex).ParentCellEntityIndex = cellEntityIndex;
    }

    private IOrderedEnumerable<KeyValuePair<int, GridPositionComponent>> SortFreeChipsEntities()
    {
      Dictionary<int, GridPositionComponent> sortedPositions = new Dictionary<int, GridPositionComponent>();
      
      foreach (int freeChipEntityIndex in _notPlacedChipsFilter)
        sortedPositions.Add(freeChipEntityIndex, _positionsPool.Get(freeChipEntityIndex));

      var sortedChips = 
        sortedPositions.OrderBy(p => p.Value.Position.y);
      
      return sortedChips;
    }
  }
}