using Gameplay.Components.Cell.Markers;
using Gameplay.Components.Chips;
using Gameplay.Components.Chips.Markers;
using Gameplay.Components.Command;
using Gameplay.Components.Common;
using Gameplay.Components.Field;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Movement
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
    private EcsPool<ChipsFieldComponent> _chipsFieldPool;
    private EcsPool<ChipComponent> _chipComponentPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _swapCombinationsFilter = _world.Filter<SwapCombinationComponent>().End();

      _swapCombinationsPool = _world.GetPool<SwapCombinationComponent>();
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();
      _placedChipsPool = _world.GetPool<PlacedChipComponent>();
      _chipsInCheckPool = _world.GetPool<ChipInCheckComponent>();
      _chipComponentPool = _world.GetPool<ChipComponent>();
      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
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
    
    private void Swap(int firstChipEntityIndex, int secondChipEntityIndex, bool isUserInitiated)
    {     
      ref ChipComponent firstChip = ref _chipComponentPool.Get(firstChipEntityIndex);
      ref ChipComponent secondChip = ref _chipComponentPool.Get(secondChipEntityIndex);

      SwapChips(ref firstChip, ref secondChip);
      DetachChips(firstChip, secondChip);
      
      if(isUserInitiated)
        MarkAsCheckNeeded(firstChip, secondChip);
    }

    private void SwapChips(ref ChipComponent firstChip, ref ChipComponent secondChip)
    {   
      ref ChipsFieldComponent chipsField = ref _chipsFieldPool.GetRawDenseItems()[1];

      ref GridPositionComponent firstChipPosition = ref _gridPositionsPool.Get(firstChip.EntityIndex);
      ref GridPositionComponent secondChipPosition = ref _gridPositionsPool.Get(secondChip.EntityIndex);

      (firstChip.ParentCellEntityIndex, secondChip.ParentCellEntityIndex) = (secondChip.ParentCellEntityIndex, firstChip.ParentCellEntityIndex);
      (firstChipPosition.Position, secondChipPosition.Position) = (secondChipPosition.Position, firstChipPosition.Position);
      
      (chipsField.Grid[firstChipPosition.Position.x, firstChipPosition.Position.y], chipsField.Grid[secondChipPosition.Position.x, secondChipPosition.Position.y])
        = (chipsField.Grid[secondChipPosition.Position.x, secondChipPosition.Position.y], chipsField.Grid[firstChipPosition.Position.x, firstChipPosition.Position.y]);
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
      chipInCheckFirstChip.RelatedChip = secondChip.EntityIndex;
      
      ref ChipInCheckComponent chipInCheckSecond = ref _chipsInCheckPool.Add(secondChip.EntityIndex);
      chipInCheckSecond.RelatedChip = firstChip.EntityIndex;
    }
  }
}