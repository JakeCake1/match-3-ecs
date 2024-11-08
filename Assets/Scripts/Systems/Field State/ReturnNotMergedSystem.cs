using Components.Chips;
using Components.Command;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public class ReturnNotMergedSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipFilter;
    
    private EcsPool<Chip> _chipsPool;
    private EcsPool<ChipInCheck> _inCheckPool;
    private EcsPool<SwapCombination> _swapCombinationPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipFilter = _world.Filter<Chip>().Inc<ChipInCheck>().End();

      _chipsPool = _world.GetPool<Chip>();
      _inCheckPool = _world.GetPool<ChipInCheck>();
      _swapCombinationPool = _world.GetPool<SwapCombination>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int chipEntityIndex in _chipFilter)
      {
        if(!_inCheckPool.Has(chipEntityIndex))
          continue;
        
        ref ChipInCheck chipInCheckChip = ref _inCheckPool.Get(chipEntityIndex);

        int swapCombinationEntity = _world.NewEntity();
        ref SwapCombination swapCombination = ref _swapCombinationPool.Add(swapCombinationEntity);
        
        swapCombination.Pair = (_chipsPool.Get(chipEntityIndex), _chipsPool.Get(chipInCheckChip.RelatedChip.ChipEntityIndex));
          
        _inCheckPool.Del(chipInCheckChip.RelatedChip.ChipEntityIndex);
        _inCheckPool.Del(chipEntityIndex);
      }
    }
  }
}