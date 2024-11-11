using Components.Chips;
using Components.Chips.Markers;
using Components.Command;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public class ReturnNotMergedSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipFilter;
    
    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<ChipInCheckComponent> _inCheckPool;
    private EcsPool<SwapCombinationComponent> _swapCombinationPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipFilter = _world.Filter<ChipComponent>().Inc<ChipInCheckComponent>().End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _inCheckPool = _world.GetPool<ChipInCheckComponent>();
      _swapCombinationPool = _world.GetPool<SwapCombinationComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int chipEntityIndex in _chipFilter)
      {
        if(!_inCheckPool.Has(chipEntityIndex))
          continue;
        
        ref ChipInCheckComponent chipInCheckChip = ref _inCheckPool.Get(chipEntityIndex);

        int swapCombinationEntity = _world.NewEntity();
        ref SwapCombinationComponent swapCombination = ref _swapCombinationPool.Add(swapCombinationEntity);
        
        swapCombination.Pair = (_chipsPool.Get(chipEntityIndex), _chipsPool.Get(chipInCheckChip.RelatedChip.EntityIndex));
          
        _inCheckPool.Del(chipInCheckChip.RelatedChip.EntityIndex);
        _inCheckPool.Del(chipEntityIndex);
      }
    }
  }
}