using System.Collections.Generic;
using Components.Cell;
using Components.Cell.Markers;
using Components.Chips;
using Components.Common;
using Components.Field;
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
    
    private EcsFilter _readyInjectorsFilter;
    private EcsFilter _notBusyCellsFilter;
    
    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<ReadyInjectorComponent> _readyInjectorsPool;
    
    public CreateChipsSystem(FieldData fieldData) => 
      _fieldData = fieldData;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
     
      _readyInjectorsFilter = _world.Filter<ChipsInjectorComponent>().Inc<ReadyInjectorComponent>().End();
      _notBusyCellsFilter = _world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<BusyCellComponent>().End();
      
      _chipsPool = _world.GetPool<ChipComponent>();
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _readyInjectorsPool = _world.GetPool<ReadyInjectorComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if(AllCellsAreBusy())
        return;
      
      foreach (int injectorEntityIndex in _readyInjectorsFilter)
      {
        CreateChip(injectorEntityIndex);
        _readyInjectorsPool.Del(injectorEntityIndex);
      }

      bool AllCellsAreBusy() => 
        _notBusyCellsFilter.GetEntitiesCount() == 0;
    }

    private void CreateChip(int injectorEntityIndex)
    {
      int chipEntityIndex = _world.NewEntity();

      ref ChipComponent chip = ref _chipsPool.Add(chipEntityIndex);
      ref GridPositionComponent chipPosition = ref _gridPositionsPool.Add(chipEntityIndex);

      chip.Type = Random.Range(0, _fieldData.ChipsCount);
      chipPosition.Position = _gridPositionsPool.Get(injectorEntityIndex).Position;
    }
  }
}