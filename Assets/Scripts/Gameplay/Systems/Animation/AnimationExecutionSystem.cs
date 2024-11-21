using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.Components.Animation;
using Gameplay.Components.Animation.Markers;
using Gameplay.Services.AnimationService;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Animation
{
  public class AnimationExecutionSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsPool<AnimationBufferComponent> _animationBufferPool;
    private EcsPool<AnimationInExecutionComponent> _animationExecutionPool;

    private int _animationBufferEntityIndex;

    private readonly IAnimationService _animationService;

    public AnimationExecutionSystem(IAnimationService animationService) =>
      _animationService = animationService;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();

      _animationBufferPool = world.GetPool<AnimationBufferComponent>();
      _animationExecutionPool = world.GetPool<AnimationInExecutionComponent>();

      CreateBuffer(world);
    }

    private void CreateBuffer(EcsWorld world)
    {
      _animationBufferEntityIndex = world.NewEntity();

      ref AnimationBufferComponent animationBufferComponent = ref _animationBufferPool.Add(_animationBufferEntityIndex);
      animationBufferComponent.Buffer = new Queue<List<AnimationCommand>>();
    }

    public void Run(IEcsSystems systems)
    {
      if (SomeAnimationRun())
        return;

      ref AnimationBufferComponent animationBufferComponent = ref _animationBufferPool.Get(_animationBufferEntityIndex);

      if (DontHaveAnimationsForRunning(ref animationBufferComponent))
        return;
      
      MarkAsInAnimationProcess();
      
      List<AnimationCommand> animationCommands = animationBufferComponent.Buffer.Dequeue();
      RunAnimation(animationCommands, OnCompleteAnimation);

      void OnCompleteAnimation() =>
        _animationExecutionPool.Del(_animationBufferEntityIndex);

      bool SomeAnimationRun() =>
        _animationExecutionPool.Has(_animationBufferEntityIndex);

      void MarkAsInAnimationProcess() =>
        _animationExecutionPool.Add(_animationBufferEntityIndex);

      bool DontHaveAnimationsForRunning(ref AnimationBufferComponent animationBufferComponent) =>
        animationBufferComponent.Buffer.Count <= 0;
    }

    private void RunAnimation(List<AnimationCommand> animationCommands, Action onCompleteAnimation)
    {
      Sequence sequence = DOTween.Sequence();

      foreach (AnimationCommand animationCommand in animationCommands)
        sequence.Join(_animationService.StartAnimation(animationCommand));

      animationCommands.Clear();
      sequence.OnComplete(onCompleteAnimation.Invoke);
    }
  }
}