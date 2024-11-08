using System.Linq;
using Components.Chips;
using Components.Command;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public class ExecuteMergeSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _ecsFilter;
    
    private EcsPool<MergeCommand> _mergeCommandPool;
    private EcsPool<ChipForDestroy> _chipForDestroyPool;
    private EcsPool<ChipInCheck> _chipInCheckPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _ecsFilter = _world.Filter<MergeCommand>().End();
      
      _mergeCommandPool = _world.GetPool<MergeCommand>();
      _chipForDestroyPool = _world.GetPool<ChipForDestroy>();
      _chipInCheckPool = _world.GetPool<ChipInCheck>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _ecsFilter)
      {
        ref MergeCommand mergeCommand = ref _mergeCommandPool.Get(commandEntityIndex);
        
        foreach (Chip mergeCommandChip in mergeCommand.Chips)
        {
          RemoveCheckComponent(mergeCommandChip);
          MarkAsReadyForDestroy(mergeCommandChip);
        }
        
        _world.DelEntity(commandEntityIndex);
      }
    }

    private void MarkAsReadyForDestroy(Chip mergeCommandChip)
    {
      if(!_chipForDestroyPool.Has(mergeCommandChip.ChipEntityIndex))
        _chipForDestroyPool.Add(mergeCommandChip.ChipEntityIndex);
    }

    private void RemoveCheckComponent(Chip mergeCommandChip)
    {
      if (_chipInCheckPool.Has(mergeCommandChip.ChipEntityIndex))
      {
        ref ChipInCheck chipInCheck = ref _chipInCheckPool.Get(mergeCommandChip.ChipEntityIndex);
            
        if (_chipInCheckPool.Has(chipInCheck.RelatedChip.ChipEntityIndex)) 
          _chipInCheckPool.Del(chipInCheck.RelatedChip.ChipEntityIndex);
            
        _chipInCheckPool.Del(mergeCommandChip.ChipEntityIndex);
      }
    }
  }
}