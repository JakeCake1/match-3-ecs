using Components.Cell;
using Components.Chips;
using Components.Command;
using Components.Common;
using Leopotam.EcsLite;

namespace Systems.Movement
{
  public class SwapSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _ecsFilter;

    private EcsPool<SwapCombination> _swapCombinationPool;

    private EcsPool<GridPosition> _gridPositionPool;
    private EcsPool<BusyCell> _busyCellPool;
    private EcsPool<PlacedChip> _placedChipPool;
    
    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _ecsFilter = _world.Filter<SwapCombination>().End();

      _swapCombinationPool = _world.GetPool<SwapCombination>();

      _gridPositionPool = _world.GetPool<GridPosition>();

      _busyCellPool = _world.GetPool<BusyCell>();
      _placedChipPool = _world.GetPool<PlacedChip>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _ecsFilter)
      {
        ref SwapCombination swapCombination = ref _swapCombinationPool.Get(commandEntityIndex);
        Swap(swapCombination.Pair.Item1, swapCombination.Pair.Item2);

        _world.DelEntity(commandEntityIndex);
      }
    }
    
    private void Swap(Chip firstChip, Chip secondChip)
    {
      (firstChip.ParentCellEntityIndex, secondChip.ParentCellEntityIndex) = (secondChip.ParentCellEntityIndex, firstChip.ParentCellEntityIndex);

      ref GridPosition firstChipPosition = ref _gridPositionPool.Get(firstChip.ChipEntityIndex);
      ref GridPosition secondChipPosition = ref _gridPositionPool.Get(secondChip.ChipEntityIndex);

      (firstChipPosition.Position, secondChipPosition.Position) = (secondChipPosition.Position, firstChipPosition.Position);

      _gridPositionPool.Get(secondChip.ChipEntityIndex);

      _busyCellPool.Del(firstChip.ParentCellEntityIndex);
      _busyCellPool.Del(secondChip.ParentCellEntityIndex);

      _placedChipPool.Del(firstChip.ChipEntityIndex);
      _placedChipPool.Del(secondChip.ChipEntityIndex);
    }
  }
}