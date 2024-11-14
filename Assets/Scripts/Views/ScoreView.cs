using TMPro;
using UnityEngine;

namespace Views
{
  public class ScoreView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Awake() => 
      SetCount(0);

    public void SetCount(int score) => 
      _scoreText.text = $"Score: {score}";
  }
}