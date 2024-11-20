using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Data
{
  [CreateAssetMenu(fileName = "Chip Injector Data", menuName = "Chips/Create Chip Injector Data", order = 0)]
  public class ChipInjectorsData : ScriptableObject
  {
    public List<Vector2Int> ChipsInjectorsPositions;
  }
}