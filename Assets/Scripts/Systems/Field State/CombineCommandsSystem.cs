using System.Collections.Generic;
using System.Linq;
using Components.Chips;
using Components.Command;
using Components.Command.Markers;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public sealed class CombineCommandsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _emptyMergeCommandsFilter;

    private EcsPool<MergeCommandComponent> _mergeCommandsPool;
    private EcsPool<EmptyCommandComponent> _emptyCommandsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _emptyMergeCommandsFilter = _world.Filter<MergeCommandComponent>().Inc<EmptyCommandComponent>().End();

      _mergeCommandsPool = _world.GetPool<MergeCommandComponent>();
      _emptyCommandsPool = _world.GetPool<EmptyCommandComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      OverwriteOverlappingCommands();
      ClearEmptyCommands();
    }

    private void OverwriteOverlappingCommands()
    {
      MergeCommandComponent[] commands = _mergeCommandsPool.GetRawDenseItems();
      
      for (var i = 0; i < commands.Length; i++)
      {
        for (var j = i + 1; j < commands.Length; j++)
        {
          if(IsCommandsChipListsNotInitialized(i, j))
            continue;
          
          if (IsTwoCommandHasOverlappedElements(i, j))
          {
            MergeOverlappedCommandsIntoFirstCommand(i, j);
            MarkSecondCommandAsEmpty(j);
          }
        }
      }

      bool IsCommandsChipListsNotInitialized(int i, int j) =>
        commands[i].Chips == null || commands[j].Chips == null;
      
      bool IsTwoCommandHasOverlappedElements(int firstCommandIndex, int secondCommandIndex) => 
        commands[firstCommandIndex].Chips.Intersect(commands[secondCommandIndex].Chips).Any();

      void MergeOverlappedCommandsIntoFirstCommand(int firstCommandIndex, int secondCommandIndex)
      {
        Queue<ChipComponent> newQueueForCommand = new Queue<ChipComponent>(commands[firstCommandIndex].Chips.Union(commands[secondCommandIndex].Chips));
        commands[firstCommandIndex].Chips = newQueueForCommand;
      }

      void MarkSecondCommandAsEmpty(int secondCommandIndex)
      {
        commands[secondCommandIndex].Chips.Clear();
        _emptyCommandsPool.Add(commands[secondCommandIndex].CommandEntityIndex);
      }
    }

    private void ClearEmptyCommands()
    {
      foreach (var commandEntityIndex in _emptyMergeCommandsFilter) 
        _world.DelEntity(commandEntityIndex);
    }
  }
}