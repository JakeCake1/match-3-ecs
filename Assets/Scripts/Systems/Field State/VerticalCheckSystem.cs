using System.Collections.Generic;
using Components.Chips;

namespace Systems.Field_State
{
  public class VerticalCheckSystem : LineCheckSystem
  {
    protected override List<Queue<Chip>> FindLineCombinations(Chip[,] chips)
    {
      List<Queue<Chip>> combinations = new List<Queue<Chip>>();

      for (int x = 0; x < chips.GetLength(0); x++)
      {
        Queue<Chip> chipsCombo = new Queue<Chip>();
        
        for (int y = 0; y < chips.GetLength(1); y++)
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

      void AddQueueToCombinationList(Queue<Chip> chipsCombo)
      {
        if (chipsCombo.Count >= 3) 
          combinations.Add(new Queue<Chip>(chipsCombo));

        chipsCombo.Clear();
      }
    }

    private void AddChipToQueue(Chip[,] chips, Queue<Chip> chipsCombo, int x, int y)
    {
      if(chips[x, y].ChipEntityIndex == 0)
        return;
      
      chipsCombo.Enqueue(chips[x, y]);
    }
  }
}