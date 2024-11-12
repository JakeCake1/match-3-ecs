using Components.Chips;
using Components.Chips.Markers;
using Components.Command;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public sealed class ReturnNotMergedSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipsInCheckProcessFilter;
    
    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<ChipInCheckComponent> _chipsInCheckProcessPool;
    private EcsPool<SwapCombinationComponent> _swapCombinationsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsInCheckProcessFilter = _world.Filter<ChipComponent>().Inc<ChipInCheckComponent>().End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _chipsInCheckProcessPool = _world.GetPool<ChipInCheckComponent>();
      _swapCombinationsPool = _world.GetPool<SwapCombinationComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int chipEntityIndex in _chipsInCheckProcessFilter)
      {
        if(ChipNotInProcessPool(chipEntityIndex))
          continue;
        
        CreateSwapCombination(chipEntityIndex);
        RemoveCheckFlagsForChips(chipEntityIndex);
      }
      
      bool ChipNotInProcessPool(int chipEntityIndex) => 
        !_chipsInCheckProcessPool.Has(chipEntityIndex);
    }

    private void CreateSwapCombination(int chipEntityIndex)
    {
      ref ChipInCheckComponent chipInCheckChip = ref _chipsInCheckProcessPool.Get(chipEntityIndex);

      int swapCombinationEntity = _world.NewEntity();
      ref SwapCombinationComponent swapCombination = ref _swapCombinationsPool.Add(swapCombinationEntity);
        
      swapCombination.Pair = (_chipsPool.Get(chipEntityIndex), _chipsPool.Get(chipInCheckChip.RelatedChip.EntityIndex));
    }

    private void RemoveCheckFlagsForChips(int chipEntityIndex)
    {
      _chipsInCheckProcessPool.Del(_chipsInCheckProcessPool.Get(chipEntityIndex).RelatedChip.EntityIndex);
      _chipsInCheckProcessPool.Del(chipEntityIndex);
    }
  }
}