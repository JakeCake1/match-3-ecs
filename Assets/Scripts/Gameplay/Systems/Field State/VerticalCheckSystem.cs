using System.Collections.Generic;
using Gameplay.Components.Chips;

namespace Gameplay.Systems.Field_State
{
  public sealed class VerticalCheckSystem : LineCheckSystem
  {
    protected override List<Queue<ChipComponent>> FindLineCombinations(ref int[,] chips)
    {
      List<Queue<ChipComponent>> combinations = new List<Queue<ChipComponent>>();
      Queue<ChipComponent> chipsCombo = new Queue<ChipComponent>();

      for (int x = 0; x < chips.GetLength(0); x++)
      {
        for (int y = 0; y < chips.GetLength(1); y++) 
          CheckChipForSequence(chips, chipsCombo, x, y, combinations);
        
        AddQueueToCombinationList(chipsCombo, combinations);
      }

      return combinations;
    }
  }
}