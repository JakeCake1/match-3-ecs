using System;
using Components.Command;
using Lean.Touch;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Control
{
  public class ControlSystem : IEcsInitSystem, IEcsDestroySystem
  {
    private EcsWorld _world;
    private EcsPool<SwapCommandComponent> _commandPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
      _commandPool = _world.GetPool<SwapCommandComponent>();

      LeanTouch.OnFingerSwipe += HandleFingerSwipe;
    }

    public void Destroy(IEcsSystems systems)
    {
      LeanTouch.OnFingerSwipe -= HandleFingerSwipe;
    }

    void HandleFingerSwipe(LeanFinger finger)
    {
      int commandEntity = _world.NewEntity();
      ref SwapCommandComponent swapCommand = ref _commandPool.Add(commandEntity);

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