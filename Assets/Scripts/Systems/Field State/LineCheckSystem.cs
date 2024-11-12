using System.Collections.Generic;
using Components.Chips;
using Components.Command;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;

namespace Systems.Field_State
{
  public abstract class LineCheckSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _chipFilter;

    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;

    private FieldComponent _field;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipFilter = _world.Filter<ChipComponent>().End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      
      var fieldPool = _world.GetPool<FieldComponent>();
      _field = fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      ChipComponent[,] chips = CreateChipsGrid();  //TODO Кэшировать фишки
      List<Queue<ChipComponent>> combinations = FindLineCombinations(chips);
      AddCombinationsToMergeBuffer(combinations);
    }

    protected abstract List<Queue<ChipComponent>> FindLineCombinations(ChipComponent[,] chips);
    
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
    
    private ChipComponent[,] CreateChipsGrid()
    {
      ChipComponent[,] chips = new ChipComponent[_field.Grid.GetLength(0), _field.Grid.GetLength(1)];

      foreach (int chipEntityIndex in _chipFilter)
      {
        ref GridPositionComponent gridPosition = ref _gridPositionsPool.Get(chipEntityIndex);
        chips[gridPosition.Position.x, gridPosition.Position.y] = _chipsPool.Get(chipEntityIndex);
      }

      return chips;
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