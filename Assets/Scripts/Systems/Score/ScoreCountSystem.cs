using Components.Score;
using Leopotam.EcsLite;

namespace Systems.Score
{
  public class ScoreCountSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _viewNeedUpdateFilter;
    
    private EcsPool<UpdateScoreView> _updatePool;
    private EcsPool<ScoreCountViewRef> _scoreCountViewPool;
    private EcsPool<ScoreCount> _scoreCountPool;

    private int _playerScoreEntity;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _viewNeedUpdateFilter = _world.Filter<ScoreCountViewRef>().Inc<UpdateScoreView>().End();
      
      _scoreCountViewPool = _world.GetPool<ScoreCountViewRef>();
      _updatePool = _world.GetPool<UpdateScoreView>();
      _scoreCountPool = _world.GetPool<ScoreCount>();

      CreateScoreCounter(_world);
    }

    private void CreateScoreCounter(EcsWorld world)
    {
      _playerScoreEntity = world.NewEntity();
      var scoreCountPool = world.GetPool<ScoreCount>();
        
      ref ScoreCount scoreCount = ref scoreCountPool.Add(_playerScoreEntity);

      scoreCount.CounterEntityIndex = _playerScoreEntity;
      scoreCount.PlayerScore = 0;

      world.GetPool<NeedCreateScoreCount>().Add(_playerScoreEntity);
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int i in _viewNeedUpdateFilter)
      { 
        ref ScoreCountViewRef scoreCountViewRef = ref _scoreCountViewPool.Get(i);
        
        scoreCountViewRef.ScoreView.SetCount(_scoreCountPool.Get(_playerScoreEntity).PlayerScore);
        
        _updatePool.Del(i);
      }
    }
  }
}