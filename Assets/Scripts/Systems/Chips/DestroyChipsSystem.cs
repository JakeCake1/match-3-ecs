using Components.Cell;
using Components.Chips;
using Components.Common;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public class DestroyChipsSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipsForDestroyFilter;
    
    private EcsPool<Chip> _chipsPool;
    private EcsPool<BusyCell> _busyCellsPool;
    
    private EcsPool<ChipViewRef> _chipViewsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsForDestroyFilter = _world.Filter<Chip>().Inc<GridPosition>().Inc<ChipViewRef>().Inc<ChipForDestroy>().End();

      _chipsPool = _world.GetPool<Chip>();
      _busyCellsPool = _world.GetPool<BusyCell>();
      
      _chipViewsPool = _world.GetPool<ChipViewRef>();
      
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