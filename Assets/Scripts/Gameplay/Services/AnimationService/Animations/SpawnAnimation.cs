using DG.Tweening;
using Gameplay.Components.Animation;
using Gameplay.Data;

namespace Gameplay.Services.AnimationService.Animations
{
  public class SpawnAnimation : BaseAnimation
  {   
    private readonly FieldAnimationData _fieldAnimationData;

    public SpawnAnimation(FieldAnimationData fieldAnimationData) => 
      _fieldAnimationData = fieldAnimationData;

    public override Tween StartAnimation(AnimationCommand animationCommand) => 
      animationCommand.TargetObject.Spawn(_fieldAnimationData.Duration, _fieldAnimationData.Ease);
  }
}