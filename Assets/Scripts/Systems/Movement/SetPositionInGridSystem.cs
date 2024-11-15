using System;
using System.Text;
using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Common;
using Components.Field;
using Components.Movement;
using Leopotam.EcsLite;
using UnityEngine;

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
    private EcsPool<CellFieldComponent> _fieldPool;
    private EcsPool<ChipsFieldComponent> _chipsFieldPool;
    
    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _notPlacedChipsFilter = _world.Filter<ChipComponent>()
        .Inc<GridPositionComponent>()
        .Exc<PlacedChipComponent>()
        .End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _positionsPool = _world.GetPool<GridPositionComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();
      _placedChipsPool = _world.GetPool<PlacedChipComponent>();
      _moveViewsCommandsPool = _world.GetPool<MoveViewCommand>();

      _fieldPool = _world.GetPool<CellFieldComponent>();
      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if (AllChipsPlaced())
        return;
      
      ref ChipsFieldComponent chipsField = ref _chipsFieldPool.GetRawDenseItems()[1];
      
      for (int y = 0; y < chipsField.Grid.GetLength(1); y++)
      {
        for (int x = 0; x < chipsField.Grid.GetLength(0); x++)
        {
          if(IsEmptyGridCell(ref chipsField.Grid[x, y]))
            continue;
          
          if(ChipIsPlaced(ref chipsField.Grid[x, y]))
            continue;
          
          ref GridPositionComponent chipPosition = ref _positionsPool.Get(chipsField.Grid[x,y].EntityIndex);
          
          int targetY = FindLowestNotBusyPositionInGrid(chipPosition, out int targetCellEntityIndex);
          MarkChipAndPlace(ref chipsField, targetY, chipsField.Grid[x,y].EntityIndex, targetCellEntityIndex);
        }
      }
      
      bool AllChipsPlaced() => 
        _notPlacedChipsFilter.GetEntitiesCount() == 0;

      bool IsEmptyGridCell(ref ChipComponent chipComponent) => 
        chipComponent.EntityIndex == -1;

      bool ChipIsPlaced(ref ChipComponent chipComponent) => 
        _placedChipsPool.Has(chipComponent.EntityIndex);
    }

    private int FindLowestNotBusyPositionInGrid(GridPositionComponent chipPosition, out int targetCellEntityIndex)
    { 
      ref CellFieldComponent cellField = ref _fieldPool.GetRawDenseItems()[1];

      int targetY = 0;
      targetCellEntityIndex = 0;

      int startY = Math.Clamp(chipPosition.Position.y, 0, cellField.Grid.GetLength(1) - 1);

      for (int y = startY; y >= 0; y--)
      {
        if (_busyCellsPool.Has(cellField.Grid[chipPosition.Position.x, y]))
          break;

        targetCellEntityIndex = cellField.Grid[chipPosition.Position.x, y];
        targetY = y;
      }

      return targetY;
    }

    private void MarkChipAndPlace(ref ChipsFieldComponent chipsField, int y, int freeChipEntityIndex, int cellEntityIndex)
    {
      ref GridPositionComponent chipPosition = ref _positionsPool.Get(freeChipEntityIndex);
      ref ChipComponent chip = ref _chipsPool.Get(freeChipEntityIndex);
      ref CellFieldComponent cellField = ref _fieldPool.GetRawDenseItems()[1];

      Vector2Int cachedPosition = chipPosition.Position;
      chipPosition.Position = _positionsPool.Get(cellField.Grid[chipPosition.Position.x, y]).Position;
      
      chipsField.Grid[cachedPosition.x, cachedPosition.y] = default;
      chipsField.Grid[cachedPosition.x, cachedPosition.y].EntityIndex = -1;
      
      chipsField.Grid[chipPosition.Position.x, chipPosition.Position.y] = chip;
      
      chip.ParentCellEntityIndex = cellEntityIndex;

      ref MoveViewCommand moveViewCommand = ref _moveViewsCommandsPool.Add(freeChipEntityIndex);
      moveViewCommand.NewViewPosition = chipPosition.Position;
      
      ref BusyCellComponent busyCellComponent = ref _busyCellsPool.Add(cellEntityIndex);
      busyCellComponent.ChipEntityIndex = freeChipEntityIndex;
      
      _placedChipsPool.Add(freeChipEntityIndex);
    }
  }
}