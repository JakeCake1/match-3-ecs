using DG.Tweening;
using Gameplay.Components.Animation;
using Gameplay.Data;

namespace Gameplay.Services.AnimationService.Animations
{
  public class DestroyAnimation : BaseAnimation
  {
    private readonly FieldAnimationData _fieldAnimationData;

    public DestroyAnimation(FieldAnimationData fieldAnimationData) => 
      _fieldAnimationData = fieldAnimationData;

    public override Tween StartAnimation(AnimationCommand animationCommand) => 
      animationCommand.TargetObject.Destroy(_fieldAnimationData.Duration, _fieldAnimationData.Ease);
  }
}