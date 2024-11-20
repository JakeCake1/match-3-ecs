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
    private const string FieldDataAddress = "Field Data";
    private const string CameraDataAddress = "Camera Data";
    private const string CellViewAddress = "Cell View";
    private const string ChipViewAddress = "Chip View";
    private const string ScoreCountViewAddress = "Score Count View";
    private const string AddScoreViewAddress = "Add Score View";
    private const string CameraAddress = "Camera";
    private const string AnimationDataAddress = "Field Animation Data";

    private FieldData _fieldData;
    private CameraData _cameraData;
    private FieldAnimationData _fieldAnimationData;

    private CellView _cellViewPrefab;
    private ChipView _chipViewPrefab;
    private ScoreView _scoreViewPrefab;
    private AddScoreView _addScoreViewPrefab;

    private Camera _camera;

    private readonly IAssetProvider _assetProvider;

    public GamePlayInstaller(IAssetProvider assetProvider) =>
      _assetProvider = assetProvider;

    public async UniTask Preload()
    {
      _fieldData = await _assetProvider.Load<FieldData>(FieldDataAddress);
      _cameraData = await _assetProvider.Load<CameraData>(CameraDataAddress);
      _fieldAnimationData = await _assetProvider.Load<FieldAnimationData>(AnimationDataAddress);

      _cellViewPrefab = (await _assetProvider.Load<GameObject>(CellViewAddress)).GetComponentInChildren<CellView>();
      _chipViewPrefab = (await _assetProvider.Load<GameObject>(ChipViewAddress)).GetComponentInChildren<ChipView>();

      _scoreViewPrefab = (await _assetProvider.Load<GameObject>(ScoreCountViewAddress)).GetComponentInChildren<ScoreView>();
      _addScoreViewPrefab = (await _assetProvider.Load<GameObject>(AddScoreViewAddress)).GetComponentInChildren<AddScoreView>();

      _camera = (Object.Instantiate(await _assetProvider.Load<GameObject>(CameraAddress))).GetComponentInChildren<Camera>();
    }

    public void Install(IContainerBuilder builder)
    {
      builder.RegisterInstance(_fieldData);
      builder.RegisterInstance(_cameraData);
      builder.RegisterInstance(_fieldAnimationData);

      builder.RegisterInstance(_cellViewPrefab);
      builder.RegisterInstance(_chipViewPrefab);
      builder.RegisterInstance(_scoreViewPrefab);
      builder.RegisterInstance(_addScoreViewPrefab);

      builder.RegisterInstance(_camera);
    }

    public void Clear()
    {
      _assetProvider.Release(FieldDataAddress);
      _assetProvider.Release(CameraDataAddress);
      _assetProvider.Release(AnimationDataAddress);

      _assetProvider.Release(CellViewAddress);
      _assetProvider.Release(ChipViewAddress);

      _assetProvider.Release(ScoreCountViewAddress);
      _assetProvider.Release(AddScoreViewAddress);

      Object.Destroy(_camera.gameObject);
      _assetProvider.Release(CameraAddress);
    }
  }
}