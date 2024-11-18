using Data;
using Leopotam.EcsLite;
using Systems.Camera;
using Systems.Chips;
using Systems.Control;
using Systems.Field_State;
using Systems.Grid;
using Systems.Injector;
using Systems.Movement;
using Systems.Score;
using Systems.Score.View_Systems;
using UnityEngine;
using VContainer;
using Views;

internal sealed class EcsStartup : MonoBehaviour
{
  private EcsWorld _world;

  private IEcsSystems _systems;
  
  private CellView _cellViewPrefab;
  private ChipView _chipViewPrefab;
  private ScoreView _scoreViewPrefab;
  private AddScoreView _addScoreViewPrefab;

  private FieldData _fieldData;

  private CameraData _cameraData;
  private Camera _camera;

  private ChipInjectorsData _chipInjectorsData;

  [Inject]
  public void Construct(CellView cellViewPrefab, FieldData fieldData, Camera camera,
    CameraData cameraData, ChipInjectorsData chipInjectorsData, ChipView chipViewPrefab,
    ScoreView scoreViewPrefab, AddScoreView addScoreViewPrefab)
  {
    _addScoreViewPrefab = addScoreViewPrefab;
    _scoreViewPrefab = scoreViewPrefab;
    _chipViewPrefab = chipViewPrefab;
    _chipInjectorsData = chipInjectorsData;
    _cameraData = cameraData;
    _camera = camera;
    _fieldData = fieldData;
    _cellViewPrefab = cellViewPrefab;
  }

  private void Start()
  {
    _world = new EcsWorld();

    _systems = new EcsSystems(_world);
    
    _systems
      .Add(new CreateFieldInitSystem(_fieldData))
      .Add(new CreateCellViewSystem(_cellViewPrefab, _fieldData))
      .Add(new CameraResizeInitSystem(_camera, _cameraData))
      .Add(new CreateChipsInjectorsInitSystem(_chipInjectorsData))
      
      //
      
      .Add(new CreateChipsSystem(_fieldData))
      .Add(new CreateChipsViewsSystem(_fieldData, _chipViewPrefab))
      .Add(new FindSwapsSystem(_camera))
      .Add(new SwapSystem())
      .Add(new SetPositionInGridSystem())
      .Add(new SetPositionForViewSystem())
      .Add(new RechargeInjectorsSystem())
      .Add(new DestroyChipsSystem())
      .Add(new DestroyChipsViewsSystem())
      .Add(new VerticalShiftSystem())
      .Add(new VerticalCheckSystem())
      .Add(new HorizontalCheckSystem())
      .Add(new CombineCommandsSystem())
      .Add(new ExecuteMergeSystem())     
      .Add(new ReturnNotMergedSystem())
      
      .Add(new ControlSystem())
      
      .Add(new ScoreCountSystem())
      .Add(new CreateScoreViewInitSystem(_scoreViewPrefab))
      .Add(new CreateAddScoreSystem())
      .Add(new CreateAddScoreViewSystem(_addScoreViewPrefab))
      
#if UNITY_EDITOR
      .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
      .Init();
  }

  private void Update() => 
    _systems?.Run();

  private void OnDestroy()
  {
    CleanupSystems();
    CleanupWorlds();
  }

  private void CleanupSystems()
  {
    if (_systems == null)
      return;

    _systems.Destroy();
    _systems = null;
  }

  private void CleanupWorlds()
  {
    if (_world == null)
      return;

    _world.Destroy();
    _world = null;
  }
}