using DG.Tweening;
using Gameplay.Components.Animation;
using Gameplay.Data;

namespace Gameplay.Services.AnimationService.Animations
{
  public class MoveAnimation : BaseAnimation
  {
    private readonly FieldAnimationData _fieldAnimationData;

    public MoveAnimation(FieldAnimationData fieldAnimationData) => 
      _fieldAnimationData = fieldAnimationData;

    public override Tween StartAnimation(AnimationCommand animationCommand) => 
      animationCommand.TargetObject.AnimateToPosition(_fieldAnimationData.Duration, _fieldAnimationData.Ease, animationCommand.TargetPosition);
  }
}