using Components.Cell;
using Components.Chips;
using Components.Common;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public class RechargeInjectorsSystem : IEcsInitSystem, IEcsRunSystem 
  {
    private EcsWorld _world;
    
    private EcsFilter _filterInjectors;
    private EcsFilter _filterCells;
    
    private EcsPool<BusyCell> _busyPool;
    private EcsPool<ReadyInjector> _readyPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _filterInjectors = _world.Filter<ChipsInjector>().Exc<ReadyInjector>().End();
      _filterCells = _world.Filter<Cell>().Inc<GridPosition>().Exc<BusyCell>().End();
      
      _busyPool = _world.GetPool<BusyCell>();
      _readyPool = _world.GetPool<ReadyInjector>();    
      
      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {   
      if(_filterCells.GetEntitiesCount() == 0)
        return;
      
      foreach (int injectorEntityIndex in _filterInjectors)
      foreach (int cellsEntityIndex in _filterCells)
      {
        if(_busyPool.Has(cellsEntityIndex))
          continue;
        
        if(!_readyPool.Has(injectorEntityIndex))
          _readyPool.Add(injectorEntityIndex);
      }
      
      Debug.Log($"Run: {GetType().Name}");
    }
  }
}