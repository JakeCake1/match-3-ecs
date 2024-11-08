using Components.Cell;
using Components.Chips;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;

namespace Systems.Chips
{
  public class PositioningForChipSystem : IEcsRunSystem
  {
    public void Run(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();

      var filterChips = world.Filter<Chip>().Inc<GridPosition>().Exc<PlacedChip>().End();

      var positionsPool = world.GetPool<GridPosition>();
      var busyCellPool = world.GetPool<BusyCell>();
      var placedChipPool = world.GetPool<PlacedChip>();
      
      var chipViewRefPool = world.GetPool<ChipViewRef>();

      var fieldPool = world.GetPool<Field>();

      Field field = fieldPool.GetRawDenseItems()[1];
      
      foreach (int entityChipIndex in filterChips)
      {
        ref GridPosition chipPosition = ref positionsPool.Get(entityChipIndex);

        for (int y = 0; y < field.Grid.GetLength(1); y++)
        {
          if(busyCellPool.Has(field.Grid[chipPosition.Position.x, y].EntityIndex))
            continue;
          
          chipPosition.Position = field.Grid[chipPosition.Position.x, y].Position;
          chipViewRefPool.Get(entityChipIndex).ChipView.SetPosition(chipPosition.Position);
            
          placedChipPool.Add(entityChipIndex);
          busyCellPool.Add(field.Grid[chipPosition.Position.x, y].EntityIndex);

          break;
        }
      }
    }
  }
}