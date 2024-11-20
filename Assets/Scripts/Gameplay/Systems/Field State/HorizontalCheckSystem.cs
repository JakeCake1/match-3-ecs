using System.Collections.Generic;
using Gameplay.Components.Chips;
using Gameplay.Components.Field;

namespace Gameplay.Systems.Field_State
{
  public sealed class HorizontalCheckSystem : LineCheckSystem
  {
    protected override List<Queue<ChipComponent>> FindLineCombinations(ref ChipsFieldComponent chipsFieldComponent)
    {
      List<Queue<ChipComponent>> combinations = new List<Queue<ChipComponent>>();
      Queue<ChipComponent> chipsCombo = new Queue<ChipComponent>();

      for (int y = 0; y < chipsFieldComponent.Grid.GetLength(1); y++)
      {
        for (int x = 0; x < chipsFieldComponent.Grid.GetLength(0); x++) 
          CheckChipForSequence(ref chipsFieldComponent, chipsCombo, x, y, combinations);
        
        AddQueueToCombinationList(chipsCombo, combinations);
      }

      return combinations;
    }
  }
}