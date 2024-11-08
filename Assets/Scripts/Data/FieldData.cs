using System;
using UnityEngine;

namespace Data
{
  [CreateAssetMenu(menuName = "Field/Create Field Data", fileName = "Field Data")]
  public class FieldData : ScriptableObject
  {
    public Vector2 Offset;
    public Vector2Int Size;
    
    public int ChipsCount;

    private void OnValidate()
    {
      if (ChipsCount > 7)
        ChipsCount = 7;
    }
  }
}