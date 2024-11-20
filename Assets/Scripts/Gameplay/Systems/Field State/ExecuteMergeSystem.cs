using Gameplay.Components.Chips;
using Gameplay.Components.Chips.Markers;
using Gameplay.Components.Command;
using Gameplay.Components.Score;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Field_State
{
  public sealed class ExecuteMergeSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _mergeCommandsFilter;

    private EcsPool<MergeCommandComponent> _mergeCommandsPool;
    private EcsPool<ChipForDestroyComponent> _chipsForDestroyPool;
    private EcsPool<ChipInCheckComponent> _chipsInCheckProcessPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _mergeCommandsFilter = _world.Filter<MergeCommandComponent>().End();

      _mergeCommandsPool = _world.GetPool<MergeCommandComponent>();
      _chipsForDestroyPool = _world.GetPool<ChipForDestroyComponent>();
      _chipsInCheckProcessPool = _world.GetPool<ChipInCheckComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _mergeCommandsFilter)
      {
        ref MergeCommandComponent mergeCommand = ref _mergeCommandsPool.Get(commandEntityIndex);
        
        AddScore(mergeCommand.Chips.Count);
        
        foreach (ChipComponent mergeCommandChip in mergeCommand.Chips)
        {
          RemoveCheckComponent(mergeCommandChip);
          MarkChipAsReadyForDestroy(mergeCommandChip);
        }
        
        _world.DelEntity(commandEntityIndex);
      }
    }

    private void MarkChipAsReadyForDestroy(ChipComponent mergeCommandChip)
    {
      if (mergeCommandChip.EntityIndex == -1)
        return;
      
      if (!_chipsForDestroyPool.Has(mergeCommandChip.EntityIndex))
        _chipsForDestroyPool.Add(mergeCommandChip.EntityIndex);
    }

    private void RemoveCheckComponent(ChipComponent mergeCommandChip)
    {
      if (mergeCommandChip.EntityIndex == -1)
        return;
      
      if (_chipsInCheckProcessPool.Has(mergeCommandChip.EntityIndex))
      {
        ref ChipInCheckComponent chipInCheck = ref _chipsInCheckProcessPool.Get(mergeCommandChip.EntityIndex);

        if (_chipsInCheckProcessPool.Has(chipInCheck.RelatedChip))
          _chipsInCheckProcessPool.Del(chipInCheck.RelatedChip);

        _chipsInCheckProcessPool.Del(mergeCommandChip.EntityIndex);
      }
    }

    private void AddScore(int chipsCount)
    {
      int addScoreEntityIndex = _world.NewEntity();
      ref AddScoreCommandComponent addScoreCommand = ref _world.GetPool<AddScoreCommandComponent>().Add(addScoreEntityIndex);
      
      addScoreCommand.CommandEntityIndex = addScoreEntityIndex;
      addScoreCommand.ScoreCount = chipsCount;
    }
  }
}