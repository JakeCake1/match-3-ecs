using Components.Chips;
using Components.Chips.Markers;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Control
{
  public class TestControlSystem : IEcsInitSystem, IEcsRunSystem
  {
    private readonly UnityEngine.Camera _camera;
    
    private EcsWorld _world;
    
    private EcsPool<ChipForDestroy> _chipsForDestroyPool;

    public TestControlSystem(UnityEngine.Camera camera) => 
      _camera = camera;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
      _chipsForDestroyPool = _world.GetPool<ChipForDestroy>();
      
      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {
      if (Input.GetMouseButtonDown(0))
      {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
        
        if (raycastHit2D)
        {
          var chipView = raycastHit2D.collider.GetComponent<ChipView>();
          if (chipView) 
            _chipsForDestroyPool.Add(chipView.Entity);
        }
        
        Debug.Log($"Run: {GetType().Name}");
      }
    }
  }
}