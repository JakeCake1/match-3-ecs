using DG.Tweening;
using Gameplay.Components.Animation;

namespace Gameplay.Services.AnimationService
{
  public interface IAnimationService
  {
    Tween StartAnimation(AnimationCommand animationCommand);
  }
}