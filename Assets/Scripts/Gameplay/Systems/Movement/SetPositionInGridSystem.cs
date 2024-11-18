using System;
using Gameplay.Components.Cell.Markers;
using Gameplay.Components.Chips;
using Gameplay.Components.Chips.Markers;
using Gameplay.Components.Common;
using Gameplay.Components.Field;
using Gameplay.Components.Movement;
using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Systems.Movement
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
          if (IsEmptyGridCell(chipsField.Grid[x, y]))
            continue;

          if (ChipIsPlaced(chipsField.Grid[x, y]))
            continue;

          ref GridPositionComponent chipPosition = ref _positionsPool.Get(chipsField.Grid[x, y]);

          int targetY = FindLowestNotBusyPositionInGrid(chipPosition, out int targetCellEntityIndex);
          MarkChipAndPlace(ref chipsField, targetY, chipsField.Grid[x, y], targetCellEntityIndex);
        }
      }

      bool AllChipsPlaced() =>
        _notPlacedChipsFilter.GetEntitiesCount() == 0;

      bool IsEmptyGridCell(int chipEntityIndex) =>
        chipEntityIndex == -1;

      bool ChipIsPlaced(int chipEntityIndex) =>
        _placedChipsPool.Has(chipEntityIndex);
    }

    private int FindLowestNotBusyPositionInGrid(GridPositionComponent chipPosition, out int targetCellEntityIndex)
    {
      ref CellFieldComponent cellField = ref _fieldPool.GetRawDenseItems()[1];

      int targetY = 0;
      targetCellEntityIndex = 0;

      targetY = IterateFieldFromTopToBottom(ref cellField, ref targetCellEntityIndex);

      return targetY;

      int IterateFieldFromTopToBottom(ref CellFieldComponent cellField, ref int targetCellEntityIndex)
      {
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
    }

    private void MarkChipAndPlace(ref ChipsFieldComponent chipsField, int y, int freeChipEntityIndex, int cellEntityIndex)
    {
      ref GridPositionComponent chipPosition = ref _positionsPool.Get(freeChipEntityIndex);
      ref ChipComponent chip = ref _chipsPool.Get(freeChipEntityIndex);

      SetupChip(ref chip, ref chipPosition);
      ModifyChipField(ref chipPosition, ref chipsField);
      CreateMoveCommandForChipView(ref chipPosition);
      MarkCellAsBusy();

      _placedChipsPool.Add(freeChipEntityIndex);

      void SetupChip(ref ChipComponent chip, ref GridPositionComponent chipPosition)
      {
        ref CellFieldComponent cellField = ref _fieldPool.GetRawDenseItems()[1];

        chip.ParentCellEntityIndex = cellEntityIndex;
        chipPosition.Position = _positionsPool.Get(cellField.Grid[chipPosition.Position.x, y]).Position;
      }

      void ModifyChipField(ref GridPositionComponent chipPosition, ref ChipsFieldComponent chipsField)
      {
        Vector2Int cachedPosition = chipPosition.Position;

        chipsField.Grid[cachedPosition.x, cachedPosition.y] = default;
        chipsField.Grid[cachedPosition.x, cachedPosition.y] = -1;
        chipsField.Grid[chipPosition.Position.x, chipPosition.Position.y] = freeChipEntityIndex;
      }

      void CreateMoveCommandForChipView(ref GridPositionComponent chipPosition)
      {
        ref MoveViewCommand moveViewCommand = ref _moveViewsCommandsPool.Add(freeChipEntityIndex);
        moveViewCommand.NewViewPosition = chipPosition.Position;
      }

      void MarkCellAsBusy()
      {
        ref BusyCellComponent busyCellComponent = ref _busyCellsPool.Add(cellEntityIndex);
        busyCellComponent.ChipEntityIndex = freeChipEntityIndex;
      }
    }
  }
}