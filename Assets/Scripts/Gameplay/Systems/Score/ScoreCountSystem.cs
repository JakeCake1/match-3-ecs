using Gameplay.Components.Score;
using Gameplay.Components.Score.Markers;
using Gameplay.Components.Score.View;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Score
{
  public sealed class ScoreCountSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _scoreViewsNeedUpdateFilter;
    
    private EcsPool<UpdateScoreComponent> _needUpdateViewsPool;
    private EcsPool<ScoreCountViewRefComponent> _scoreCountViewsPool;
    private EcsPool<ScoreCountComponent> _scoreCountPool;

    private int _playerScoreEntityIndex;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _scoreViewsNeedUpdateFilter = _world.Filter<ScoreCountViewRefComponent>().Inc<UpdateScoreComponent>().End();
      
      _scoreCountViewsPool = _world.GetPool<ScoreCountViewRefComponent>();
      _needUpdateViewsPool = _world.GetPool<UpdateScoreComponent>();
      _scoreCountPool = _world.GetPool<ScoreCountComponent>();

      CreateScoreCounter(_world);
    }

    private void CreateScoreCounter(EcsWorld world)
    {
      _playerScoreEntityIndex = world.NewEntity();
      
      var scoreCountPool = world.GetPool<ScoreCountComponent>();
        
      ref ScoreCountComponent scoreCount = ref scoreCountPool.Add(_playerScoreEntityIndex);

      scoreCount.CounterEntityIndex = _playerScoreEntityIndex;
      scoreCount.PlayerScore = 0;

      world.GetPool<NeedCreateScoreCountComponent>().Add(_playerScoreEntityIndex);
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int scoreViewEntityIndex in _scoreViewsNeedUpdateFilter)
      {
        UpdateScoreView(scoreViewEntityIndex);
        _needUpdateViewsPool.Del(scoreViewEntityIndex);
      }
    }

    private void UpdateScoreView(int scoreViewEntityIndex)
    {
      ref ScoreCountViewRefComponent scoreCountViewRef = ref _scoreCountViewsPool.Get(scoreViewEntityIndex);
      int playerScore = _scoreCountPool.Get(_playerScoreEntityIndex).PlayerScore;
      scoreCountViewRef.ScoreView.SetCount(playerScore);
    }
  }
}