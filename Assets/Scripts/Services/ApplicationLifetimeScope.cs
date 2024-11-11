using Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Views;

namespace Services
{
  public class ApplicationLifetimeScope : LifetimeScope
  {
    [SerializeField] private CellView _cellViewPrefab;
    [SerializeField] private FieldData _fieldData;
    [SerializeField] private Camera _camera;
    [SerializeField] private CameraData _cameraData;
    [SerializeField] private ChipInjectorsData _chipInjectorsData;
    [SerializeField] private ChipView _chipViewPrefab;
    [SerializeField] private ScoreView _scoreViewPrefab;
    [SerializeField] private AddScoreView _addScoreViewPrefab;
    
    protected override void Configure(IContainerBuilder builder)
    {      
      builder.RegisterInstance(_cellViewPrefab);
      builder.RegisterInstance(_fieldData);
      builder.RegisterInstance(_camera);
      builder.RegisterInstance(_cameraData);
      builder.RegisterInstance(_chipInjectorsData);
      builder.RegisterInstance(_chipViewPrefab);
      builder.RegisterInstance(_scoreViewPrefab);
      builder.RegisterInstance(_addScoreViewPrefab);
    }
  }
}