using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.Views
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
    
    public int EntityIndex { get; private set; }

    public void Construct(int entity, Vector2 fieldDataOffset)
    {
      EntityIndex = entity;
      _fieldDataOffset = fieldDataOffset;  
      
      transform.localScale = Vector3.zero;
    }

    public void SetPosition(Vector2Int chipPosition) => 
      transform.position = chipPosition + _fieldDataOffset * chipPosition;

    public Vector2 GetPosition() =>
      transform.position;
    
    public void SetType(int chipType) => 
      SpriteRenderer.color = _chipsColors[chipType];

    public float GetSize() => 
      (transform.localScale.x + transform.localScale.y + transform.localScale.z)/3;

    public Tween Spawn()
    {
      return transform.DOScale(0.75f, .2f)
        .SetEase(Ease.Linear)
        .SetAutoKill();
    }

    public Tween Destroy()
    {
      return transform.DOScale(0, .2f)
        .SetEase(Ease.Linear)
        .SetAutoKill()
        .OnComplete(() => Destroy(gameObject));
    }

    public Tween AnimateToPosition(Vector2Int chipPosition)
    {
      return transform.DOMove(chipPosition + _fieldDataOffset * chipPosition, .2f)
        .SetEase(Ease.Linear)
        .SetAutoKill();
    }
  }
}