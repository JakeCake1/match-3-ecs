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
    private EcsPool<GridPosition> _gridPool;
    private EcsPool<ChipViewRef> _chipViewsPool;
    private EcsPool<ChipForDestroy> _chipsForDestroyPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _chipsForDestroyFilter = _world.Filter<Chip>().Inc<GridPosition>().Inc<ChipViewRef>().Inc<ChipForDestroy>().End();

      _chipsPool = _world.GetPool<Chip>();
      _gridPool = _world.GetPool<GridPosition>();
      _chipViewsPool = _world.GetPool<ChipViewRef>();
      _chipsForDestroyPool = _world.GetPool<ChipForDestroy>();
      
      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {
      if(_chipsForDestroyFilter.GetEntitiesCount() == 0)
        return;
      
      foreach (int chipEntity in _chipsForDestroyFilter)
      {
        _chipsForDestroyPool.Del(chipEntity);
        
        _chipViewsPool.Get(chipEntity).ChipView.Destroy();
        _chipViewsPool.Del(chipEntity);
        
        _gridPool.Del(chipEntity);
        _chipsPool.Del(chipEntity);
      }
      
      Debug.Log($"Run: {GetType().Name}");
    }

    public void Destroy(IEcsSystems systems) => 
      Debug.Log($"Destroy: {GetType().Name}");
  }
}