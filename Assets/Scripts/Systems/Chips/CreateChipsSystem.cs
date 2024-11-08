using Components.Cell;
using Components.Chips;
using Components.Common;
using Leopotam.EcsLite;

namespace Systems.Chips
{
  public class CreateChipsSystem : IEcsRunSystem
  {
    public void Run(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();
     
      var filterChipsInjector = world.Filter<ChipsInjector>().Inc<ReadyInjector>().End();
      var filterCell = world.Filter<Cell>().Inc<GridPosition>().Exc<BusyCell>().End();
      
      var chipsPool = world.GetPool<Chip>();
      
      var gridPositionPool = world.GetPool<GridPosition>();
      var chipsInjectorPool = world.GetPool<ReadyInjector>();

      if(filterCell.GetEntitiesCount() == 0)
        return;
      
      foreach (int injectorEntityIndex in filterChipsInjector)
      {
        if (chipsInjectorPool.Has(injectorEntityIndex))
        {
          int chipEntity = world.NewEntity();

          chipsPool.Add(chipEntity);
          ref GridPosition chipPosition = ref gridPositionPool.Add(chipEntity);

          chipPosition.Position = gridPositionPool.Get(injectorEntityIndex).Position;

          chipsInjectorPool.Del(injectorEntityIndex);
        }
      }
    }
  }
}