using System.Collections.Generic;
using Components.Chips;

namespace Systems.Field_State
{
  public sealed class HorizontalCheckSystem : LineCheckSystem
  {
    protected override List<Queue<ChipComponent>> FindLineCombinations(ChipComponent[,] chips)
    {
      List<Queue<ChipComponent>> combinations = new List<Queue<ChipComponent>>();
      Queue<ChipComponent> chipsCombo = new Queue<ChipComponent>();

      for (int y = 0; y < chips.GetLength(1); y++)
      {
        for (int x = 0; x < chips.GetLength(0); x++) 
          CheckChipForSequence(chips, chipsCombo, x, y, combinations);
        
        AddQueueToCombinationList(chipsCombo, combinations);
      }

      return combinations;
    }
  }
}