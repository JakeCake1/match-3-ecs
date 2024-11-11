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

    private Field _field;

    private EcsPool<Chip> _chipsPool;
    private EcsPool<GridPosition> _gridPositionsPool;

    private EcsFilter _chipFilter;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipFilter = _world.Filter<Chip>().End();

      _chipsPool = _world.GetPool<Chip>();
      _gridPositionsPool = _world.GetPool<GridPosition>();
      
      var fieldPool = _world.GetPool<Field>();
      _field = fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      Chip[,] chips = CreateChipsGrid();
      List<Queue<Chip>> combinations = FindLineCombinations(chips);
      AddCombinationsToMergeBuffer(combinations);
    }

    private Chip[,] CreateChipsGrid()
    {
      Chip[,] chips = new Chip[_field.Grid.GetLength(0), _field.Grid.GetLength(1)];

      foreach (int chipEntityIndex in _chipFilter)
      {
        ref GridPosition gridPosition = ref _gridPositionsPool.Get(chipEntityIndex);
        chips[gridPosition.Position.x, gridPosition.Position.y] = _chipsPool.Get(chipEntityIndex);
      }

      return chips;
    }

    protected abstract List<Queue<Chip>> FindLineCombinations(Chip[,] chips);

    private void AddCombinationsToMergeBuffer(List<Queue<Chip>> combinations)
    {
      foreach (Queue<Chip> combination in combinations)
      {
        int mergeCommandEntity = _world.NewEntity();
        ref MergeCommand mergeCommand = ref _world.GetPool<MergeCommand>().Add(mergeCommandEntity);
        
        mergeCommand.CommandEntityIndex = mergeCommandEntity;
        mergeCommand.Chips = combination;
      }

      combinations.Clear();
    }
  }
}