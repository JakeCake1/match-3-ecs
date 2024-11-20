using Gameplay.Views;
using UnityEngine;

namespace Gameplay.Components.Animation
{
  public struct AnimationCommand
  {
    public AnimationType Type;
    public ChipView TargetObject;
    public Vector2Int TargetPosition;
  }
}