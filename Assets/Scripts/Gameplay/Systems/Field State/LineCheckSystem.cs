using System.Collections.Generic;
using Gameplay.Components.Chips;
using Gameplay.Components.Command;
using Gameplay.Components.Field;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Field_State
{
  public abstract class LineCheckSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsPool<ChipsFieldComponent> _chipsFieldPool;
    private EcsPool<ChipComponent> _chipsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
      _chipsPool = _world.GetPool<ChipComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      ref ChipsFieldComponent chipsFieldComponent = ref _chipsFieldPool.GetRawDenseItems()[1];

      List<Queue<ChipComponent>> combinations = FindLineCombinations(ref chipsFieldComponent);
      AddCombinationsToMergeBuffer(combinations);
    }

    protected abstract List<Queue<ChipComponent>> FindLineCombinations(ref ChipsFieldComponent chipsFieldComponent);

    protected void CheckChipForSequence(ref ChipsFieldComponent chips, Queue<ChipComponent> chipsCombo, int x, int y, List<Queue<ChipComponent>> combinations)
    {
      if (chipsCombo.Count == 0)
        AddChipToQueue(ref chips, chipsCombo, x, y);
      else
      {
        if (ChipIsNotInitialized(chips.Grid[x, y]) || NextChipInLineIsDifferent(chips.Grid[x, y]))
          AddQueueToCombinationList(chipsCombo, combinations);

        AddChipToQueue(ref chips, chipsCombo, x, y);
      }

      bool ChipIsNotInitialized(int chipEntityIndex) => 
        chipEntityIndex == -1;

      bool NextChipInLineIsDifferent(int chipEntityIndex) => 
        chipsCombo.Peek().Type != _chipsPool.Get(chipEntityIndex).Type;
    }

    protected void AddQueueToCombinationList(Queue<ChipComponent> chipsCombo, List<Queue<ChipComponent>> combinations)
    {
      if (chipsCombo.Count >= 3)
        combinations.Add(new Queue<ChipComponent>(chipsCombo));

      chipsCombo.Clear();
    }

    private void AddChipToQueue(ref ChipsFieldComponent chipsFieldComponent, Queue<ChipComponent> chipsCombo, int x, int y)
    {
      if (ChipIsNotInitialized(chipsFieldComponent.Grid[x, y]))
        return;
      
      chipsCombo.Enqueue(_chipsPool.Get(chipsFieldComponent.Grid[x, y]));
      
      bool ChipIsNotInitialized(int chipEntityIndex) => 
        chipEntityIndex == -1;
    }

    private void AddCombinationsToMergeBuffer(List<Queue<ChipComponent>> combinations)
    {
      foreach (Queue<ChipComponent> combination in combinations)
        CreateMergeCommand(combination);

      combinations.Clear();
    }

    private void CreateMergeCommand(Queue<ChipComponent> combination)
    {
      int mergeCommandEntity = _world.NewEntity();
      ref MergeCommandComponent mergeCommand = ref _world.GetPool<MergeCommandComponent>().Add(mergeCommandEntity);

      mergeCommand.CommandEntityIndex = mergeCommandEntity;
      mergeCommand.Chips = combination;
    }
  }
}