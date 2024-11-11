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
    
    private EcsPool<AddScoreCommandComponent> _scoreCommandsPool;
    private EcsPool<ShowAddedScoreCountComponent> _showAddedScoreViewPool;
    
    private EcsPool<ScoreCountComponent> _scoreCountPool;
    private EcsPool<UpdateScoreComponent> _updateScoreViewPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _addScoreCommandsFilter = _world.Filter<AddScoreCommandComponent>().End();

      _scoreCommandsPool = _world.GetPool<AddScoreCommandComponent>();
      _showAddedScoreViewPool = _world.GetPool<ShowAddedScoreCountComponent>();

      _scoreCountPool = _world.GetPool<ScoreCountComponent>();
      _updateScoreViewPool = _world.GetPool<UpdateScoreComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _addScoreCommandsFilter)
      {
        ref AddScoreCommandComponent addScoreCommand = ref _scoreCommandsPool.Get(commandEntityIndex);
        ref ScoreCountComponent scoreCount = ref _scoreCountPool.GetRawDenseItems()[1];
        
        scoreCount.PlayerScore += ScoreModifier * addScoreCommand.ScoreCount;
      
        if(!_updateScoreViewPool.Has(scoreCount.CounterEntityIndex))
          _updateScoreViewPool.Add(scoreCount.CounterEntityIndex);

        CreateAddScoreViewEntity(addScoreCommand);

        _world.DelEntity(commandEntityIndex);
      }
    }

    private void CreateAddScoreViewEntity(AddScoreCommandComponent addScoreCommand)
    {
      int addScoreViewEntityIndex = _world.NewEntity();
      ref ShowAddedScoreCountComponent showAddedScoreCount = ref _showAddedScoreViewPool.Add(addScoreViewEntityIndex);
      showAddedScoreCount.AddedScore = ScoreModifier * addScoreCommand.ScoreCount;
    }
  }
}