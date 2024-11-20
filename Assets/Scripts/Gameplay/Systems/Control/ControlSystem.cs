using System;
using Gameplay.Components.Animation;
using Gameplay.Components.Animation.Markers;
using Gameplay.Components.Command;
using Lean.Touch;
using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Systems.Control
{
  public sealed class ControlSystem : IEcsInitSystem, IEcsDestroySystem
  {
    private EcsWorld _world;
    
    private EcsPool<SwapCommandComponent> _swapCommandsPool;
    private EcsFilter _animationInExecutionFilter;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
      
      _swapCommandsPool = _world.GetPool<SwapCommandComponent>();
      _animationInExecutionFilter = _world.Filter<AnimationInExecutionComponent>().End();

      LeanTouch.OnFingerSwipe += HandleFingerSwipe;
    }

    public void Destroy(IEcsSystems systems) => 
      LeanTouch.OnFingerSwipe -= HandleFingerSwipe;

    void HandleFingerSwipe(LeanFinger finger)
    {
      if(_animationInExecutionFilter.GetEntitiesCount() > 0)
        return;
      
      int commandEntity = _world.NewEntity();
      ref SwapCommandComponent swapCommand = ref _swapCommandsPool.Add(commandEntity);

      swapCommand.Ray = (finger.StartScreenPosition, GetDirection(finger));
    }

    Vector2Int GetDirection(LeanFinger leanFinger)
    {
      Vector2 direction = leanFinger.LastScreenPosition - leanFinger.StartScreenPosition;
      direction = direction.normalized;
      
      return new Vector2Int((int)Math.Round(direction.x), (int)Math.Round(direction.y));
    }
  }
}