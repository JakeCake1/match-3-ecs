using System.Collections.Generic;
using System.Linq;
using Components.Chips;
using Components.Command;
using Components.Command.Markers;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public class CombineCommandsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _emptyMergeCommandsFilter;

    private EcsPool<MergeCommandComponent> _mergeCommandPool;
    private EcsPool<EmptyCommandComponent> _emptyCommandPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _emptyMergeCommandsFilter = _world.Filter<MergeCommandComponent>().Inc<EmptyCommandComponent>().End();

      _mergeCommandPool = _world.GetPool<MergeCommandComponent>();
      _emptyCommandPool = _world.GetPool<EmptyCommandComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      OverwriteOverlappingCommands();
      ClearEmptyCommands();
    }

    private void OverwriteOverlappingCommands()
    {
      MergeCommandComponent[] commands = _mergeCommandPool.GetRawDenseItems();
      
      for (var i = 0; i < commands.Length; i++)
      {
        for (var j = i + 1; j < commands.Length; j++)
        {
          if(commands[i].Chips == null || commands[j].Chips == null)
            continue;
          
          if (IsTwoCommandHasOverlappedElements(i, j))
          {
            MergeOverlappedCommandsIntoFirstCommand(i, j);
            MarkSecondCommandAsEmpty(j);
          }
        }
      }

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
        _emptyCommandPool.Add(commands[secondCommandIndex].CommandEntityIndex);
      }
    }

    private void ClearEmptyCommands()
    {
      foreach (var commandEntityIndex in _emptyMergeCommandsFilter) 
        _world.DelEntity(commandEntityIndex);
    }
  }
}