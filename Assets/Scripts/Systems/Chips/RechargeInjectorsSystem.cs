using Components.Cell;
using Components.Chips;
using Components.Common;
using Leopotam.EcsLite;

namespace Systems.Chips
{
  public class RechargeInjectorsSystem : IEcsRunSystem 
  {
    public void Run(IEcsSystems systems)
    {   
      EcsWorld world = systems.GetWorld();

      var filterInjectors = world.Filter<ChipsInjector>().Exc<ReadyInjector>().End();
      var filterCells = world.Filter<Cell>().Inc<GridPosition>().Exc<BusyCell>().End();
      
      var busyPool = world.GetPool<BusyCell>();
      var readyPool = world.GetPool<ReadyInjector>();
      
      foreach (int injectorEntityIndex in filterInjectors)
      foreach (int cellsEntityIndex in filterCells)
      {
        if(busyPool.Has(cellsEntityIndex))
          continue;
        
        if(!readyPool.Has(injectorEntityIndex))
          readyPool.Add(injectorEntityIndex);
      }
    }
  }
}