using Data;
using Leopotam.EcsLite;
using Systems.Camera;
using Systems.Grid;
using UnityEngine;
using VContainer;
using Views;

internal sealed class EcsStartup : MonoBehaviour
{
  private EcsWorld _world;
  private IEcsSystems _systems;
  
  private CellView _cellViewPrefab;
  private FieldData _fieldData;
  
  private CameraData _cameraData;
  private Camera _camera;

  [Inject]
  public void Construct(CellView cellViewPrefab, FieldData fieldData, Camera camera, CameraData cameraData)
  {
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
    if (_systems != null)
    {
      _systems.Destroy();
      _systems = null;
    }
  }

  private void CleanupWorlds()
  {
    if (_world != null)
    {
      _world.Destroy();
      _world = null;
    }
  }
}