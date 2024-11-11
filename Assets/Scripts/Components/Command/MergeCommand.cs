using System.Collections.Generic;
using Components.Chips;

namespace Components.Command
{
  public struct MergeCommand
  {
    public int CommandEntityIndex;
    public Queue<Chip> Chips;
  }
}