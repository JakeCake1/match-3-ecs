using System.Collections.Generic;

namespace Gameplay.Components.Animation
{
  public struct AnimationBufferComponent
  {
    public Queue<List<AnimationCommand>> Buffer;
  }
}