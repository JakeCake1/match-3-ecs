using DG.Tweening;
using Gameplay.Components.Animation;

namespace Gameplay.Services.AnimationService.Animations
{
  public abstract class BaseAnimation
  {
    public abstract Tween StartAnimation(AnimationCommand animationCommand);
  }
}