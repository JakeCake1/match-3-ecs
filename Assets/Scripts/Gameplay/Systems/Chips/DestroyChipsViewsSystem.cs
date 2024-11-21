using System.Collections.Generic;
using Gameplay.Components.Animation;
using Gameplay.Components.Chips.Markers;
using Gameplay.Services.AnimationService;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Chips
{
  public class DestroyChipsViewsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipsViewsForDestroyFilter;
    
    private EcsPool<ChipViewForDestroyComponent> _chipsViewsForDestroyPool;
    private EcsPool<AnimationBufferComponent> _animationBufferPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
      
      _chipsViewsForDestroyFilter = _world.Filter<ChipViewForDestroyComponent>().End();
      
      _chipsViewsForDestroyPool = _world.GetPool<ChipViewForDestroyComponent>();  
      _animationBufferPool = _world.GetPool<AnimationBufferComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if(DontHaveViewsForDestroy())
        return;
      
      List<AnimationCommand> animationCommands = new List<AnimationCommand>();

      foreach (int commandEntityIndex in _chipsViewsForDestroyFilter) 
        DestroyChipView(animationCommands, commandEntityIndex);
      
      _animationBufferPool.GetRawDenseItems()[1].Buffer.Enqueue(animationCommands);

      bool DontHaveViewsForDestroy() => 
        _chipsViewsForDestroyFilter.GetEntitiesCount() <= 0;
    }

    private void DestroyChipView(List<AnimationCommand> animationCommands, int commandEntityIndex)
    {
      ref ChipViewForDestroyComponent chipForDestroyComponent = ref _chipsViewsForDestroyPool.Get(commandEntityIndex);

      PushAnimationCommandIntoBuffer(ref chipForDestroyComponent);
      DestroyChipViewDeleteCommand();
      
      void DestroyChipViewDeleteCommand() => 
        _world.DelEntity(commandEntityIndex);

      void PushAnimationCommandIntoBuffer(ref ChipViewForDestroyComponent chipForDestroyComponent)
      {
        animationCommands.Add(new AnimationCommand
        {
          Type = AnimationType.Destroy,
          TargetObject = chipForDestroyComponent.ChipView,
        });
      }
    }
  }
}