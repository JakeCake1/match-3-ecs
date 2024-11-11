using Components.Score;
using Components.Score.Markers;
using Components.Score.View;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Score.View_Systems
{
  public class CreateScoreViewSystem : IEcsInitSystem
  {
    private readonly ScoreView _scoreViewPrefab;
    
    public CreateScoreViewSystem(ScoreView scoreViewPrefab) => 
      _scoreViewPrefab = scoreViewPrefab;

    public void Init(IEcsSystems systems) => 
      CreateScoreViewCounter(systems.GetWorld());

    private void CreateScoreViewCounter(EcsWorld world)
    {
      var scoreCountPool = world.GetPool<ScoreCountViewRefComponent>();
      var needCreateViewPool = world.GetPool<NeedCreateScoreCountComponent>();
      
      foreach (int scoreCountEntityIndex in world.Filter<ScoreCountComponent>().Inc<NeedCreateScoreCountComponent>().End())
      {     
        ref ScoreCountViewRefComponent scoreCountViewRef = ref scoreCountPool.Add(scoreCountEntityIndex);
        scoreCountViewRef.ScoreView = Object.Instantiate(_scoreViewPrefab.gameObject).GetComponent<ScoreView>();
          
        needCreateViewPool.Del(scoreCountEntityIndex);
      }
    }
  }
}