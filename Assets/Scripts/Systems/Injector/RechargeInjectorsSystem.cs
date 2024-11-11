using Components.Cell;
using Components.Cell.Markers;
using Components.Chips;
using Components.Common;
using Components.Injector;
using Components.Injector.Markers;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Injector
{
  public class RechargeInjectorsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _filterInjectors;
    private EcsFilter _filterCells;

    private EcsPool<BusyCellComponent> _busyPool;
    private EcsPool<ReadyInjectorComponent> _readyPool;
    
    private EcsPool<GridPositionComponent> _gridPositionPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _filterInjectors = _world.Filter<ChipsInjectorComponent>().Exc<ReadyInjectorComponent>().End();
      _filterCells = _world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<BusyCellComponent>().End();

      _gridPositionPool = _world.GetPool<GridPositionComponent>();
      _busyPool = _world.GetPool<BusyCellComponent>();
      _readyPool = _world.GetPool<ReadyInjectorComponent>();

      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {
      if (_filterCells.GetEntitiesCount() == 0)
        return;

      foreach (int cellsEntityIndex in _filterCells)
      {
        if (_busyPool.Has(cellsEntityIndex))
          continue;

        foreach (int injectorEntityIndex in _filterInjectors)
        {
          if (_gridPositionPool.Get(injectorEntityIndex).Position.x == _gridPositionPool.Get(cellsEntityIndex).Position.x)
          {
            if (_readyPool.Has(injectorEntityIndex))
              continue;
            
            _readyPool.Add(injectorEntityIndex);
            break;
          }
        }
      }

      Debug.Log($"Run: {GetType().Name}");
    }
  }
}