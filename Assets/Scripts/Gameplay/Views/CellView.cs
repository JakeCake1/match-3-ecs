using UnityEngine;

namespace Gameplay.Views
{
  public class CellView : MonoBehaviour
  {
    public Vector2 GetPosition() => 
      transform.position;
  }
}