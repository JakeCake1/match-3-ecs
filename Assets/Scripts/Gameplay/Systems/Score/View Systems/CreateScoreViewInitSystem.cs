using Gameplay.Components.Score;
using Gameplay.Components.Score.Markers;
using Gameplay.Components.Score.View;
using Gameplay.Views;
using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Systems.Score.View_Systems
{
  public sealed class CreateScoreViewInitSystem : IEcsInitSystem
  {
    private readonly ScoreView _scoreViewPrefab;

    private EcsFilter _scoreViewsNeedCreationFilter;
    
    private EcsPool<ScoreCountViewRefComponent> _scoreCountsPool;
    private EcsPool<NeedCreateScoreCountComponent> _scoreViewsNeedCreationPool;

    public CreateScoreViewInitSystem(ScoreView scoreViewPrefab) => 
      _scoreViewPrefab = scoreViewPrefab;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();
      
      _scoreViewsNeedCreationFilter = world.Filter<ScoreCountComponent>().Inc<NeedCreateScoreCountComponent>().End();

      _scoreCountsPool = world.GetPool<ScoreCountViewRefComponent>();
      _scoreViewsNeedCreationPool = world.GetPool<NeedCreateScoreCountComponent>();

      CreateScoreViewCounter();
    }

    private void CreateScoreViewCounter()
    {
      foreach (int scoreCountEntityIndex in _scoreViewsNeedCreationFilter)
      {
        CreateScoreView(scoreCountEntityIndex);
        _scoreViewsNeedCreationPool.Del(scoreCountEntityIndex);
      }
    }

    private void CreateScoreView(int scoreCountEntityIndex)
    {
      ref ScoreCountViewRefComponent scoreCountViewRef = ref _scoreCountsPool.Add(scoreCountEntityIndex);
      scoreCountViewRef.ScoreView = Object.Instantiate(_scoreViewPrefab.gameObject).GetComponent<ScoreView>();
    }
  }
}