using Components.Score.Markers;
using Components.Score.View;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Score.View_Systems
{
  public class CreateAddScoreViewSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private readonly AddScoreView _addScoreViewPrefab;

    private EcsFilter _addedScoresFilter;

    private EcsPool<ShowAddedScoreCountComponent> _addedScoresPool;
    private EcsPool<AddScoreViewRefComponent> _addScoreViewRefsPool;
    
    private int _addScoreViewEntityIndex;

    public CreateAddScoreViewSystem(AddScoreView addScoreViewPrefab) => 
      _addScoreViewPrefab = addScoreViewPrefab;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
      
      _addedScoresFilter = _world.Filter<ShowAddedScoreCountComponent>().End();
      
      _addedScoresPool = _world.GetPool<ShowAddedScoreCountComponent>();
      _addScoreViewRefsPool = _world.GetPool<AddScoreViewRefComponent>();
      
      CreateAddScoreView();
    }

    private void CreateAddScoreView()
    {
      _addScoreViewEntityIndex = _world.NewEntity();

      ref AddScoreViewRefComponent addScoreViewRef = ref _addScoreViewRefsPool.Add(_addScoreViewEntityIndex);
      addScoreViewRef.AddScoreView = Object.Instantiate(_addScoreViewPrefab);
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int addedScoreEntityIndex in _addedScoresFilter)
      {
        ref AddScoreViewRefComponent addScoreViewRef = ref _addScoreViewRefsPool.Get(_addScoreViewEntityIndex);
        addScoreViewRef.AddScoreView.SetCount(_addedScoresPool.Get(addedScoreEntityIndex).AddedScore);
        
        _world.DelEntity(addedScoreEntityIndex);
      }
    }
  }
}