using Components.Cell;
using Components.Chips;
using Components.Common;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public class CreateChipsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private readonly FieldData _fieldData;
    
    private EcsWorld _world;
    
    private EcsFilter _filterChipsInjector;
    private EcsFilter _filterCell;
    
    private EcsPool<Chip> _chipsPool;
    private EcsPool<GridPosition> _gridPositionPool;
    private EcsPool<ReadyInjector> _chipsInjectorPool;

    public CreateChipsSystem(FieldData fieldData) => 
      _fieldData = fieldData;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
     
      _filterChipsInjector = _world.Filter<ChipsInjector>().Inc<ReadyInjector>().End();
      _filterCell = _world.Filter<Cell>().Inc<GridPosition>().Exc<BusyCell>().End();
      
      _chipsPool = _world.GetPool<Chip>();
      
      _gridPositionPool = _world.GetPool<GridPosition>();
      _chipsInjectorPool = _world.GetPool<ReadyInjector>();   
      
      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {
      if(_filterCell.GetEntitiesCount() == 0)
        return;
      
      foreach (int injectorEntityIndex in _filterChipsInjector)
      {
        if (_chipsInjectorPool.Has(injectorEntityIndex))
        {
          int chipEntity = _world.NewEntity();

          ref Chip chip = ref _chipsPool.Add(chipEntity);
          ref GridPosition chipPosition = ref _gridPositionPool.Add(chipEntity);

          chip.Type = Random.Range(0, _fieldData.ChipsCount);
          chipPosition.Position = _gridPositionPool.Get(injectorEntityIndex).Position;

          _chipsInjectorPool.Del(injectorEntityIndex);
        }
      }
      
      Debug.Log($"Run: {GetType().Name}");
    }
  }
}