using System.Collections.Generic;
using UnityEngine;

namespace Views
{
  public class ChipView : MonoBehaviour
  {
    [SerializeField] private SpriteRenderer SpriteRenderer;

    private readonly Dictionary<int, Color> _chipsColors = new()
    {
      {0, Color.blue},
      {1, Color.green},
      {2, Color.red},
      {3, Color.yellow},
      {4, Color.cyan},
      {5, Color.magenta},
      {6, Color.gray}
    };
    
    private Vector2 _fieldDataOffset;
    
    public int Entity { get; private set; }

    public void Construct(int entity, Vector2 fieldDataOffset)
    {
      Entity = entity;
      _fieldDataOffset = fieldDataOffset;
    }

    public void SetPosition(Vector2Int chipPosition) => 
      transform.position = chipPosition + _fieldDataOffset * chipPosition;

    public void SetType(int chipType) => 
      SpriteRenderer.color = _chipsColors[chipType];

    public void Destroy() => 
      Destroy(gameObject);
  }
}