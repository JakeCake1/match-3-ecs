using System.Text;
using Components.Chips;
using Components.Chips.Markers;
using Components.Command;
using Components.Score;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Field_State
{
  public class ExecuteMergeSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _ecsFilter;

    private EcsPool<MergeCommandComponent> _mergeCommandPool;
    private EcsPool<ChipForDestroyComponent> _chipForDestroyPool;
    private EcsPool<ChipInCheckComponent> _chipInCheckPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _ecsFilter = _world.Filter<MergeCommandComponent>().End();

      _mergeCommandPool = _world.GetPool<MergeCommandComponent>();
      _chipForDestroyPool = _world.GetPool<ChipForDestroyComponent>();
      _chipInCheckPool = _world.GetPool<ChipInCheckComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _ecsFilter)
      {
        ref MergeCommandComponent mergeCommand = ref _mergeCommandPool.Get(commandEntityIndex);

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"ExecuteMergeSystem: command index {mergeCommand.CommandEntityIndex} destroy {mergeCommand.Chips.Count} chips");
     
        foreach (ChipComponent chip in mergeCommand.Chips) 
          stringBuilder.AppendLine($"Chip: chip index {chip.EntityIndex} type {chip.Type}");

        Debug.Log(stringBuilder.ToString());

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
      if (!_chipForDestroyPool.Has(mergeCommandChip.EntityIndex))
        _chipForDestroyPool.Add(mergeCommandChip.EntityIndex);
    }

    private void RemoveCheckComponent(ChipComponent mergeCommandChip)
    {
      if (_chipInCheckPool.Has(mergeCommandChip.EntityIndex))
      {
        ref ChipInCheckComponent chipInCheck = ref _chipInCheckPool.Get(mergeCommandChip.EntityIndex);

        if (_chipInCheckPool.Has(chipInCheck.RelatedChip.EntityIndex))
          _chipInCheckPool.Del(chipInCheck.RelatedChip.EntityIndex);

        _chipInCheckPool.Del(mergeCommandChip.EntityIndex);
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