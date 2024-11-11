using TMPro;
using UnityEngine;

namespace Views
{
  public class AddScoreView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _addScoreText;
    
    public void SetCount(int score) => 
      _addScoreText.text = score.ToString();
  }
}