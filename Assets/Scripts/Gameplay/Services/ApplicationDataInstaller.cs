using Gameplay.Data;
using Gameplay.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Services
{
  public sealed class ApplicationDataInstaller : LifetimeScope
  {
    [SerializeField] private FieldData _fieldData;
    [SerializeField] private ChipInjectorsData _chipInjectorsData;
    
    [SerializeField] private CellView _cellViewPrefab;
    [SerializeField] private ChipView _chipViewPrefab;
    [SerializeField] private ScoreView _scoreViewPrefab;
    [SerializeField] private AddScoreView _addScoreViewPrefab;
    
    [SerializeField] private Camera _camera;
    [SerializeField] private CameraData _cameraData;

    protected override void Configure(IContainerBuilder builder)
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
  }
}