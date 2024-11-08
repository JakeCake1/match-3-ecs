using Components.Cell;
using Components.Chips;
using Components.Common;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public class CreateChipsSystem : IEcsRunSystem
  {
    private FieldData _fieldData;

    public CreateChipsSystem(FieldData fieldData) => 
      _fieldData = fieldData;

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

          ref Chip chip = ref chipsPool.Add(chipEntity);
          ref GridPosition chipPosition = ref gridPositionPool.Add(chipEntity);

          chip.Type = Random.Range(0, _fieldData.ChipsCount);
          chipPosition.Position = gridPositionPool.Get(injectorEntityIndex).Position;

          chipsInjectorPool.Del(injectorEntityIndex);
        }
      }
    }
  }
}