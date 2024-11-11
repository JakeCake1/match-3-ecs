using Components.Cell;
using Components.Cell.Markers;
using Components.Chips;
using Components.Chips.Markers;
using Components.Common;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public class DestroyChipsSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipsForDestroyFilter;
    
    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<BusyCellComponent> _busyCellsPool;
    
    private EcsPool<ChipViewRefComponent> _chipViewsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsForDestroyFilter = _world.Filter<ChipComponent>().Inc<GridPositionComponent>().Inc<ChipViewRefComponent>().Inc<ChipForDestroyComponent>().End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();
      
      _chipViewsPool = _world.GetPool<ChipViewRefComponent>();
      
      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {
      if(_chipsForDestroyFilter.GetEntitiesCount() == 0)
        return;
      
      foreach (int chipEntity in _chipsForDestroyFilter)
      {       
        _chipViewsPool.Get(chipEntity).ChipView.Destroy();
        _busyCellsPool.Del(_chipsPool.Get(chipEntity).ParentCellEntityIndex);
        
        _world.DelEntity(chipEntity);
      }
      
      Debug.Log($"Run: {GetType().Name}");
    }

    public void Destroy(IEcsSystems systems) => 
      Debug.Log($"Destroy: {GetType().Name}");
  }
}