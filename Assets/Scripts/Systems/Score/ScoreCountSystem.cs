using Components.Score;
using Components.Score.Markers;
using Components.Score.View;
using Leopotam.EcsLite;

namespace Systems.Score
{
  public class ScoreCountSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _viewNeedUpdateFilter;
    
    private EcsPool<UpdateScoreComponent> _updatePool;
    private EcsPool<ScoreCountViewRefComponent> _scoreCountViewPool;
    private EcsPool<ScoreCountComponent> _scoreCountPool;

    private int _playerScoreEntity;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _viewNeedUpdateFilter = _world.Filter<ScoreCountViewRefComponent>().Inc<UpdateScoreComponent>().End();
      
      _scoreCountViewPool = _world.GetPool<ScoreCountViewRefComponent>();
      _updatePool = _world.GetPool<UpdateScoreComponent>();
      _scoreCountPool = _world.GetPool<ScoreCountComponent>();

      CreateScoreCounter(_world);
    }

    private void CreateScoreCounter(EcsWorld world)
    {
      _playerScoreEntity = world.NewEntity();
      var scoreCountPool = world.GetPool<ScoreCountComponent>();
        
      ref ScoreCountComponent scoreCount = ref scoreCountPool.Add(_playerScoreEntity);

      scoreCount.CounterEntityIndex = _playerScoreEntity;
      scoreCount.PlayerScore = 0;

      world.GetPool<NeedCreateScoreCountComponent>().Add(_playerScoreEntity);
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int scoreViewEntityIndex in _viewNeedUpdateFilter)
      { 
        ref ScoreCountViewRefComponent scoreCountViewRef = ref _scoreCountViewPool.Get(scoreViewEntityIndex);
        
        scoreCountViewRef.ScoreView.SetCount(_scoreCountPool.Get(_playerScoreEntity).PlayerScore);
        
        _updatePool.Del(scoreViewEntityIndex);
      }
    }
  }
}