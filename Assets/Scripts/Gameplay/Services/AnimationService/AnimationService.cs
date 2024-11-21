using System.Collections.Generic;
using DG.Tweening;
using Gameplay.Components.Animation;
using Gameplay.Data;
using Gameplay.Services.AnimationService.Animations;

namespace Gameplay.Services.AnimationService
{
  public class AnimationService : IAnimationService
  {
    private readonly Dictionary<AnimationType, BaseAnimation> _animations;

    public AnimationService(FieldAnimationData fieldAnimationData)
    {
      _animations = new Dictionary<AnimationType, BaseAnimation>
      {
        {AnimationType.Spawn, new SpawnAnimation(fieldAnimationData)},
        {AnimationType.Move, new MoveAnimation(fieldAnimationData)},
        {AnimationType.Destroy, new DestroyAnimation(fieldAnimationData)},
      };
    }

    public Tween StartAnimation(AnimationCommand animationCommand) => 
      _animations[animationCommand.Type].StartAnimation(animationCommand);
  }
}