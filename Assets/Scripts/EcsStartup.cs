using Data;
using Leopotam.EcsLite;
using Nenuacho.EcsLite.IntervalSystems;
using Systems.Camera;
using Systems.Chips;
using Systems.Control;
using Systems.Field_State;
using Systems.Grid;
using Systems.Injector;
using Systems.Movement;
using UnityEngine;
using VContainer;
using Views;

internal sealed class EcsStartup : MonoBehaviour
{
  private EcsWorld _world;
  
  private IEcsSystems _systems;
  private EcsRunSystemsWithInterval _systemsWithInterval;

  private CellView _cellViewPrefab;  
  private ChipView _chipViewPrefab;

  private FieldData _fieldData;
  
  private CameraData _cameraData;
  private Camera _camera;
  
  private ChipInjectorsData _chipInjectorsData;

  [Inject]
  public void Construct(CellView cellViewPrefab, FieldData fieldData, Camera camera,
    CameraData cameraData, ChipInjectorsData chipInjectorsData, ChipView chipViewPrefab)
  {
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
      .Add (new CreateFieldSystem(_fieldData))
      .Add (new CreateCellViewSystem(_cellViewPrefab, _fieldData))
      .Add (new CameraResizeSystem(_camera, _cameraData))
      .Add (new CreateChipsInjectorsSystem(_chipInjectorsData))
      
      .Add (new CreateChipsSystem(_fieldData))
      .Add (new CreateChipsViewsSystem(_fieldData, _chipViewPrefab))
      .Add (new FindSwapsSystem(_camera))
      .Add (new SwapSystem())
      .Add (new SetPositionInGridSystem())
      .Add (new RechargeInjectorsSystem())
      .Add (new DestroyChipsSystem())
      .Add (new VerticalShiftSystem())
      .Add (new ControlSystem())
#if UNITY_EDITOR
      .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
      .Init();
    
    _systemsWithInterval = new EcsRunSystemsWithInterval(_world);
    _systemsWithInterval
      .Add(new ReturnNotMergedSystem(), 1f)
      .Init();
  }

  private void Update()
  {
    _systems?.Run();
    _systemsWithInterval?.Run(Time.deltaTime);
  }

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