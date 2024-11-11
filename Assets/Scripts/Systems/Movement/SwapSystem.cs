using Components.Cell;
using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Command;
using Components.Common;
using Leopotam.EcsLite;

namespace Systems.Movement
{
  public class SwapSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _ecsFilter;

    private EcsPool<SwapCombinationComponent> _swapCombinationPool;

    private EcsPool<GridPositionComponent> _gridPositionPool;
    private EcsPool<BusyCellComponent> _busyCellPool;
    private EcsPool<PlacedChipComponent> _placedChipPool;
    
    private EcsPool<ChipInCheckComponent> _inCheckPool;
    
    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _ecsFilter = _world.Filter<SwapCombinationComponent>().End();

      _swapCombinationPool = _world.GetPool<SwapCombinationComponent>();

      _gridPositionPool = _world.GetPool<GridPositionComponent>();

      _inCheckPool = _world.GetPool<ChipInCheckComponent>();
      _busyCellPool = _world.GetPool<BusyCellComponent>();
      _placedChipPool = _world.GetPool<PlacedChipComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _ecsFilter)
      {
        ref SwapCombinationComponent swapCombination = ref _swapCombinationPool.Get(commandEntityIndex);
        Swap(swapCombination.Pair.Item1, swapCombination.Pair.Item2, swapCombination.IsUserInitiated);

        _world.DelEntity(commandEntityIndex);
      }
    }
    
    private void Swap(ChipComponent firstChip, ChipComponent secondChip, bool isUserInitiated)
    {
      ref GridPositionComponent firstChipPosition = ref _gridPositionPool.Get(firstChip.EntityIndex);
      ref GridPositionComponent secondChipPosition = ref _gridPositionPool.Get(secondChip.EntityIndex);
      
      (firstChip.ParentCellEntityIndex, secondChip.ParentCellEntityIndex) = (secondChip.ParentCellEntityIndex, firstChip.ParentCellEntityIndex);
      (firstChipPosition.Position, secondChipPosition.Position) = (secondChipPosition.Position, firstChipPosition.Position);
      
      DetachChips(firstChip, secondChip);
      
      if(isUserInitiated)
        MarkAsCheckNeeded(firstChip, secondChip);
    }

    private void DetachChips(ChipComponent firstChip, ChipComponent secondChip)
    {
      _busyCellPool.Del(firstChip.ParentCellEntityIndex);
      _busyCellPool.Del(secondChip.ParentCellEntityIndex);

      _placedChipPool.Del(firstChip.EntityIndex);
      _placedChipPool.Del(secondChip.EntityIndex);
    }

    private void MarkAsCheckNeeded(ChipComponent firstChip, ChipComponent secondChip)
    {
      ref ChipInCheckComponent chipInCheckFirstChip = ref _inCheckPool.Add(firstChip.EntityIndex);
      chipInCheckFirstChip.RelatedChip = secondChip;
      
      ref ChipInCheckComponent chipInCheckSecond = ref _inCheckPool.Add(secondChip.EntityIndex);
      chipInCheckSecond.RelatedChip = firstChip;
    }
  }
}