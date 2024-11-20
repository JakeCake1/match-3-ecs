using AssetProvider.Scripts;
using Cysharp.Threading.Tasks;
using Gameplay.Data;
using Gameplay.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GamePlayInstaller : IInstaller
  {
    private FieldData _fieldData;
    private ChipInjectorsData _chipInjectorsData;

    private CellView _cellViewPrefab;
    private ChipView _chipViewPrefab;
    private ScoreView _scoreViewPrefab;
    private AddScoreView _addScoreViewPrefab;

    private Camera _camera;
    private CameraData _cameraData;
    
    private readonly IAssetProvider _assetProvider;

    public GamePlayInstaller(IAssetProvider assetProvider) => 
      _assetProvider = assetProvider;

    public async UniTask Preload()
    {
      _fieldData = await _assetProvider.Load<FieldData>("Field Data");
      _chipInjectorsData = await _assetProvider.Load<ChipInjectorsData>("Chip Injector Data");     
      _cameraData = await _assetProvider.Load<CameraData>("Camera Data");

      _cellViewPrefab = (await _assetProvider.Load<GameObject>("Cell View")).GetComponentInChildren<CellView>();
      _chipViewPrefab = (await _assetProvider.Load<GameObject>("Chip View")).GetComponentInChildren<ChipView>();
      
      _scoreViewPrefab = (await _assetProvider.Load<GameObject>("Score Count View")).GetComponentInChildren<ScoreView>();
      _addScoreViewPrefab = (await _assetProvider.Load<GameObject>("Add Score View")).GetComponentInChildren<AddScoreView>();
      
      _camera = (Object.Instantiate(await _assetProvider.Load<GameObject>("Camera"))).GetComponentInChildren<Camera>();
    }
    
    public void Install(IContainerBuilder builder)
    {
      builder.RegisterInstance(_fieldData);
      builder.RegisterInstance(_chipInjectorsData);

      builder.RegisterInstance(_cellViewPrefab);
      builder.RegisterInstance(_chipViewPrefab);
      builder.RegisterInstance(_scoreViewPrefab);
      builder.RegisterInstance(_addScoreViewPrefab);     
      
      builder.RegisterInstance(_camera);
      builder.RegisterInstance(_cameraData);
    }

    public void Clear()
    {
      _assetProvider.Release("Field Data");
      _assetProvider.Release("Chip Injector Data");     
      _assetProvider.Release("Camera Data");

      _assetProvider.Release("Cell View");
      _assetProvider.Release("Chip View");
      
      _assetProvider.Release("Score Count View");
      _assetProvider.Release("Add Score View");
      
      Object.Destroy(_camera.gameObject);
      _assetProvider.Release("Camera");
    }
  }
}