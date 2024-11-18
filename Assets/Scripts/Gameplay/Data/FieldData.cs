using UnityEngine;

namespace Gameplay.Data
{
  [CreateAssetMenu(menuName = "Field/Create Field Data", fileName = "Field Data")]
  public class FieldData : ScriptableObject
  {
    private const int MaxChipsCount = 7;
    
    public Vector2 Offset;
    public Vector2Int Size;
    
    public int ChipsCount;

    private void OnValidate()
    {
      if (ChipsCount > MaxChipsCount)
        ChipsCount = MaxChipsCount;
    }
  }
}