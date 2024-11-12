using System.Collections.Generic;
using Components.Chips;

namespace Components.Command
{
  public struct MergeCommandComponent
  {
    public int CommandEntityIndex;
    public Queue<ChipComponent> Chips;
  }
}