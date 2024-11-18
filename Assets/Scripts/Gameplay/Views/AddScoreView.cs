using TMPro;
using UnityEngine;

namespace Gameplay.Views
{
  public class AddScoreView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _addScoreText;

    private void Awake() => 
      Clean();

    public void SetCount(int score)
    {
      _addScoreText.text = $"Add score +{score}";
      
      CancelInvoke(nameof(Clean));
      Invoke(nameof(Clean), 1f);
    }

    private void Clean() => 
      _addScoreText.text = "";
  }
}