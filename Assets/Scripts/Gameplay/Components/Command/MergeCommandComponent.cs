using System.Collections.Generic;
using Gameplay.Components.Chips;

namespace Gameplay.Components.Command
{
  public struct MergeCommandComponent
  {
    public int CommandEntityIndex;
    public Queue<ChipComponent> Chips;
  }
}