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

    private FieldComponent _field;

    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;

    private EcsFilter _chipFilter;

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
      ChipComponent[,] chips = CreateChipsGrid();
      List<Queue<ChipComponent>> combinations = FindLineCombinations(chips);
      AddCombinationsToMergeBuffer(combinations);
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

    protected abstract List<Queue<ChipComponent>> FindLineCombinations(ChipComponent[,] chips);

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