using System.Collections.Generic;
using System.Linq;
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
  public class SetPositionInGridSystem : IEcsInitSystem, IEcsRunSystem
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
    private StringBuilder _stringBuilder;

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
      if (_filterChips.GetEntitiesCount() == 0)
        return;
      
      _stringBuilder = new StringBuilder();
      _stringBuilder.AppendLine("Move chips at this turn:");

      IOrderedEnumerable<KeyValuePair<int, GridPosition>> sortedChips = SortFreeChipsEntities();

      foreach (var freeChip in sortedChips)
      {
        ref GridPosition chipPosition = ref _positionsPool.Get(freeChip.Key);

        int targetY = 0;
        int targetCellEntityIndex = 0;

        int startY = chipPosition.Position.y > _field.Grid.GetLength(1) - 1 ? 
          _field.Grid.GetLength(1) - 1 : 
          chipPosition.Position.y;

        for (int y = startY; y >= 0; y--)
        {
          if (_busyCellPool.Has(_field.Grid[chipPosition.Position.x, y].EntityIndex))
            break;

          targetCellEntityIndex = _field.Grid[chipPosition.Position.x, y].EntityIndex;
          targetY = y;
        }

        MarkChipAndPlace(targetY, freeChip.Key, targetCellEntityIndex);
      }
      
      Debug.Log(_stringBuilder.ToString());
    }

    private void MarkChipAndPlace(int y, int freeChipEntityIndex, int cellEntityIndex)
    {
      ref GridPosition chipPosition = ref _positionsPool.Get(freeChipEntityIndex);
   
      _stringBuilder.AppendLine($"Move {freeChipEntityIndex} to {cellEntityIndex} old: {chipPosition.Position} new: {_field.Grid[chipPosition.Position.x, y].Position}");

      chipPosition.Position = _field.Grid[chipPosition.Position.x, y].Position;
      _chipViewRefPool.Get(freeChipEntityIndex).ChipView.SetPosition(chipPosition.Position);

      _placedChipPool.Add(freeChipEntityIndex);
      
      ref BusyCell busyCell = ref _busyCellPool.Add(cellEntityIndex);
      busyCell.ChipEntityIndex = freeChipEntityIndex;
      
      _chipPool.Get(freeChipEntityIndex).ParentCellEntityIndex = cellEntityIndex;
    }

    private IOrderedEnumerable<KeyValuePair<int, GridPosition>> SortFreeChipsEntities()
    {
      Dictionary<int, GridPosition> sortedPositions = new Dictionary<int, GridPosition>();
      
      foreach (int freeChipEntityIndex in _filterChips)
        sortedPositions.Add(freeChipEntityIndex, _positionsPool.Get(freeChipEntityIndex));

      IOrderedEnumerable<KeyValuePair<int, GridPosition>> sortedChips = sortedPositions.OrderBy(p => p.Value.Position.y);
      return sortedChips;
    }
  }
}