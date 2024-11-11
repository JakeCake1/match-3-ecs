using TMPro;
using UnityEngine;

namespace Views
{
  public class ScoreView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void SetCount(int score) => 
      _scoreText.text = score.ToString();
  }
}