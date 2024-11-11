using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Command;
using Components.Common;
using Leopotam.EcsLite;

namespace Systems.Movement
{
  public sealed class SwapSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _swapCombinationsFilter;

    private EcsPool<SwapCombinationComponent> _swapCombinationsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<BusyCellComponent> _busyCellsPool;
    private EcsPool<PlacedChipComponent> _placedChipsPool;
    private EcsPool<ChipInCheckComponent> _chipsInCheckPool;
    
    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _swapCombinationsFilter = _world.Filter<SwapCombinationComponent>().End();

      _swapCombinationsPool = _world.GetPool<SwapCombinationComponent>();
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();
      _placedChipsPool = _world.GetPool<PlacedChipComponent>();
      _chipsInCheckPool = _world.GetPool<ChipInCheckComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _swapCombinationsFilter)
      {
        ref SwapCombinationComponent swapCombination = ref _swapCombinationsPool.Get(commandEntityIndex);
        Swap(swapCombination.Pair.Item1, swapCombination.Pair.Item2, swapCombination.IsUserInitiated);
        _world.DelEntity(commandEntityIndex);
      }
    }
    
    private void Swap(ChipComponent firstChip, ChipComponent secondChip, bool isUserInitiated)
    {
      ref GridPositionComponent firstChipPosition = ref _gridPositionsPool.Get(firstChip.EntityIndex);
      ref GridPositionComponent secondChipPosition = ref _gridPositionsPool.Get(secondChip.EntityIndex);
      
      (firstChip.ParentCellEntityIndex, secondChip.ParentCellEntityIndex) = (secondChip.ParentCellEntityIndex, firstChip.ParentCellEntityIndex);
      (firstChipPosition.Position, secondChipPosition.Position) = (secondChipPosition.Position, firstChipPosition.Position);
      
      DetachChips(firstChip, secondChip);
      
      if(isUserInitiated)
        MarkAsCheckNeeded(firstChip, secondChip);
    }

    private void DetachChips(ChipComponent firstChip, ChipComponent secondChip)
    {
      _busyCellsPool.Del(firstChip.ParentCellEntityIndex);
      _busyCellsPool.Del(secondChip.ParentCellEntityIndex);

      _placedChipsPool.Del(firstChip.EntityIndex);
      _placedChipsPool.Del(secondChip.EntityIndex);
    }

    private void MarkAsCheckNeeded(ChipComponent firstChip, ChipComponent secondChip)
    {
      ref ChipInCheckComponent chipInCheckFirstChip = ref _chipsInCheckPool.Add(firstChip.EntityIndex);
      chipInCheckFirstChip.RelatedChip = secondChip;
      
      ref ChipInCheckComponent chipInCheckSecond = ref _chipsInCheckPool.Add(secondChip.EntityIndex);
      chipInCheckSecond.RelatedChip = firstChip;
    }
  }
}