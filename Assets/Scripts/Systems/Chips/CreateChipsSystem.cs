using Components.Cell;
using Components.Cell.Markers;
using Components.Chips;
using Components.Common;
using Components.Injector;
using Components.Injector.Markers;
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
    
    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionPool;
    private EcsPool<ReadyInjectorComponent> _chipsInjectorPool;

    public CreateChipsSystem(FieldData fieldData) => 
      _fieldData = fieldData;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
     
      _filterChipsInjector = _world.Filter<ChipsInjectorComponent>().Inc<ReadyInjectorComponent>().End();
      _filterCell = _world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<BusyCellComponent>().End();
      
      _chipsPool = _world.GetPool<ChipComponent>();
      
      _gridPositionPool = _world.GetPool<GridPositionComponent>();
      _chipsInjectorPool = _world.GetPool<ReadyInjectorComponent>();   
      
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

          ref ChipComponent chip = ref _chipsPool.Add(chipEntity);
          ref GridPositionComponent chipPosition = ref _gridPositionPool.Add(chipEntity);

          chip.Type = Random.Range(0, _fieldData.ChipsCount);
          chipPosition.Position = _gridPositionPool.Get(injectorEntityIndex).Position;

          _chipsInjectorPool.Del(injectorEntityIndex);
        }
      }
      
      Debug.Log($"Run: {GetType().Name}");
    }
  }
}