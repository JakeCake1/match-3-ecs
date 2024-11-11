using Components.Command;
using Components.Score;
using Leopotam.EcsLite;

namespace Systems.Score
{
  public class AddScoreSystem : IEcsInitSystem, IEcsRunSystem
  {
    private const int ScoreModifier = 10;
    
    private EcsWorld _world;
    
    private EcsFilter _addScoreCommandsFilter;
    
    private EcsPool<AddScoreCommand> _scoreCommandsPool;
    private EcsPool<ScoreCount> _scoreCountPool;
    
    private int _playerScoreEntity;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _addScoreCommandsFilter = _world.Filter<AddScoreCommand>().End();
      _scoreCommandsPool = _world.GetPool<AddScoreCommand>();

      CreateScoreCounter();

      void CreateScoreCounter()
      {
        _playerScoreEntity = _world.NewEntity();
        _scoreCountPool = _world.GetPool<ScoreCount>();
        
        _scoreCountPool.Add(_playerScoreEntity);
      }
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _addScoreCommandsFilter)
      {
        ref AddScoreCommand addScoreCommand = ref _scoreCommandsPool.Get(commandEntityIndex);
        ref ScoreCount scoreCount = ref _scoreCountPool.Get(_playerScoreEntity);
        
        scoreCount.PlayerScore += ScoreModifier * addScoreCommand.ScoreCount;
        
        _world.DelEntity(commandEntityIndex);
      }
    }
  }
}