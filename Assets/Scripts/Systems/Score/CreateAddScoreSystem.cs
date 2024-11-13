using Components.Score;
using Components.Score.Markers;
using Leopotam.EcsLite;

namespace Systems.Score
{
  public sealed class CreateAddScoreSystem : IEcsInitSystem, IEcsRunSystem
  {
    private const int ScoreModifier = 10;
    
    private EcsWorld _world;
    
    private EcsFilter _addScoreCommandsFilter;
    
    private EcsPool<AddScoreCommandComponent> _scoreCommandsPool;
    private EcsPool<ShowAddedScoreCountComponent> _showAddedScoreViewsPool;
    
    private EcsPool<ScoreCountComponent> _scoreCountPool;
    private EcsPool<UpdateScoreComponent> _updateScoreViewsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _addScoreCommandsFilter = _world.Filter<AddScoreCommandComponent>().End();

      _scoreCommandsPool = _world.GetPool<AddScoreCommandComponent>();
      _showAddedScoreViewsPool = _world.GetPool<ShowAddedScoreCountComponent>();

      _scoreCountPool = _world.GetPool<ScoreCountComponent>();
      _updateScoreViewsPool = _world.GetPool<UpdateScoreComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _addScoreCommandsFilter)
      {
        ref AddScoreCommandComponent addScoreCommand = ref _scoreCommandsPool.Get(commandEntityIndex);
        ref ScoreCountComponent scoreCount = ref _scoreCountPool.GetRawDenseItems()[1];
        
        UpdatePlayerScore(ref scoreCount, ref addScoreCommand);
        CreateAddScoreViewEntity(ref addScoreCommand);

        _world.DelEntity(commandEntityIndex);
      }
    }

    private void UpdatePlayerScore(ref ScoreCountComponent scoreCount, ref AddScoreCommandComponent addScoreCommand)
    {
      scoreCount.PlayerScore += ScoreModifier * addScoreCommand.ScoreCount;

      if(!_updateScoreViewsPool.Has(scoreCount.CounterEntityIndex))
        _updateScoreViewsPool.Add(scoreCount.CounterEntityIndex);
    }

    private void CreateAddScoreViewEntity(ref AddScoreCommandComponent addScoreCommand)
    {
      int addScoreViewEntityIndex = _world.NewEntity();
      ref ShowAddedScoreCountComponent showAddedScoreCount = ref _showAddedScoreViewsPool.Add(addScoreViewEntityIndex);
      
      showAddedScoreCount.AddedScore = ScoreModifier * addScoreCommand.ScoreCount;
    }
  }
}