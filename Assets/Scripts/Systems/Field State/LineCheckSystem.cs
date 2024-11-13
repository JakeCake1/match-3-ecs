using System.Collections.Generic;
using Components.Chips;
using Components.Command;
using Components.Field;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public abstract class LineCheckSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsPool<ChipsFieldComponent> _chipsFieldPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      ref ChipsFieldComponent chipsFieldComponent = ref _chipsFieldPool.GetRawDenseItems()[1];
      
      List<Queue<ChipComponent>> combinations = FindLineCombinations(ref chipsFieldComponent.Grid);
      AddCombinationsToMergeBuffer(combinations);
    }

    protected abstract List<Queue<ChipComponent>> FindLineCombinations(ref ChipComponent[,] chips);
    
    protected void CheckChipForSequence(ChipComponent[,] chips, Queue<ChipComponent> chipsCombo, int x, int y, List<Queue<ChipComponent>> combinations)
    {
      if (chipsCombo.Count == 0)
        AddChipToQueue(chips, chipsCombo, x, y);
      else
      {
        if (chipsCombo.Peek().Type != chips[x, y].Type) 
          AddQueueToCombinationList(chipsCombo, combinations);

        AddChipToQueue(chips, chipsCombo, x, y);
      }
    }

    protected void AddQueueToCombinationList(Queue<ChipComponent> chipsCombo, List<Queue<ChipComponent>> combinations)
    {
      if (chipsCombo.Count >= 3) 
        combinations.Add(new Queue<ChipComponent>(chipsCombo));

      chipsCombo.Clear();
    }

    private void AddChipToQueue(ChipComponent[,] chips, Queue<ChipComponent> chipsCombo, int x, int y)
    {
      if(chips[x, y].EntityIndex == 0)
        return;
      
      chipsCombo.Enqueue(chips[x, y]);
    }

    private void AddCombinationsToMergeBuffer(List<Queue<ChipComponent>> combinations)
    {
      foreach (Queue<ChipComponent> combination in combinations)
      {
        int mergeCommandEntity = _world.NewEntity();
        ref MergeCommandComponent mergeCommand = ref _world.GetPool<MergeCommandComponent>().Add(mergeCommandEntity);
        
        mergeCommand.CommandEntityIndex = mergeCommandEntity;
        mergeCommand.Chips = combination;
      }

      combinations.Clear();
    }
  }
}