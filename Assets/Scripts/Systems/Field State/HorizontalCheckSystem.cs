using System.Collections.Generic;
using Components.Chips;

namespace Systems.Field_State
{
  public class HorizontalCheckSystem : LineCheckSystem
  {
    protected override List<Queue<ChipComponent>> FindLineCombinations(ChipComponent[,] chips)
    {
      List<Queue<ChipComponent>> combinations = new List<Queue<ChipComponent>>();

      for (int y = 0; y < chips.GetLength(1); y++)
      {
        Queue<ChipComponent> chipsCombo = new Queue<ChipComponent>();
        
        for (int x = 0; x < chips.GetLength(0); x++)
        {
          if (chipsCombo.Count == 0)
            AddChipToQueue(chips, chipsCombo, x, y);
          else
          {
            if (chipsCombo.Peek().Type != chips[x, y].Type) 
              AddQueueToCombinationList(chipsCombo);

            AddChipToQueue(chips, chipsCombo, x, y);
          }
        }
        
        AddQueueToCombinationList(chipsCombo);
      }

      return combinations;

      void AddQueueToCombinationList(Queue<ChipComponent> chipsCombo)
      {
        if (chipsCombo.Count >= 3) 
          combinations.Add(new Queue<ChipComponent>(chipsCombo));

        chipsCombo.Clear();
      }
    }
    
    private void AddChipToQueue(ChipComponent[,] chips, Queue<ChipComponent> chipsCombo, int x, int y)
    {
      if(chips[x, y].EntityIndex == 0)
        return;
      
      chipsCombo.Enqueue(chips[x, y]);
    }
  }
}