using Components.Command;
using Components.Score;
using Components.Score.Markers;
using Leopotam.EcsLite;

namespace Systems.Score
{
  public class CreateAddScoreSystem : IEcsInitSystem, IEcsRunSystem
  {
    private const int ScoreModifier = 10;
    
    private EcsWorld _world;
    
    private EcsFilter _addScoreCommandsFilter;
    
    private EcsPool<AddScoreCommand> _scoreCommandsPool;
    private EcsPool<ShowAddedScoreCount> _showAddedScoreViewPool;
    
    private EcsPool<ScoreCount> _scoreCountPool;
    private EcsPool<UpdateScore> _updateScoreViewPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _addScoreCommandsFilter = _world.Filter<AddScoreCommand>().End();

      _scoreCommandsPool = _world.GetPool<AddScoreCommand>();
      _showAddedScoreViewPool = _world.GetPool<ShowAddedScoreCount>();

      _scoreCountPool = _world.GetPool<ScoreCount>();
      _updateScoreViewPool = _world.GetPool<UpdateScore>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _addScoreCommandsFilter)
      {
        ref AddScoreCommand addScoreCommand = ref _scoreCommandsPool.Get(commandEntityIndex);
        ref ScoreCount scoreCount = ref _scoreCountPool.GetRawDenseItems()[1];
        
        scoreCount.PlayerScore += ScoreModifier * addScoreCommand.ScoreCount;
      
        if(!_updateScoreViewPool.Has(scoreCount.CounterEntityIndex))
          _updateScoreViewPool.Add(scoreCount.CounterEntityIndex);

        CreateAddScoreViewEntity(addScoreCommand);

        _world.DelEntity(commandEntityIndex);
      }
    }

    private void CreateAddScoreViewEntity(AddScoreCommand addScoreCommand)
    {
      int addScoreViewEntityIndex = _world.NewEntity();
      ref ShowAddedScoreCount showAddedScoreCount = ref _showAddedScoreViewPool.Add(addScoreViewEntityIndex);
      showAddedScoreCount.AddedScore = ScoreModifier * addScoreCommand.ScoreCount;
    }
  }
}