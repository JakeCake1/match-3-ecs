using UnityEngine;

namespace Data
{
  [CreateAssetMenu(menuName = "Field/Create Field Data", fileName = "Field Data")]
  public class FieldData : ScriptableObject
  {
    public Vector2 Offset;
    public Vector2Int Size;
  }
}