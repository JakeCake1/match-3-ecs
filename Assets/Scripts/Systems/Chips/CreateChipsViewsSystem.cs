using Components.Chips;
using Components.Common;
using Data;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Chips
{
  public class CreateChipsViewsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private readonly ChipView _chipViewPrefab;
    private readonly FieldData _fieldData;
    
    private EcsWorld _world;
    
    private EcsFilter _filterChips;
    
    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionPool;
    private EcsPool<ChipViewRefComponent> _chipViewRefPool;
    
    private Transform _chipsParent;

    public CreateChipsViewsSystem(FieldData fieldData, ChipView chipViewPrefab)
    {
      _fieldData = fieldData;
      _chipViewPrefab = chipViewPrefab;
    }

    public void Init(IEcsSystems systems)
    {
      _chipsParent = new GameObject("Chips").transform;
      
      _world = systems.GetWorld();

      _filterChips = _world.Filter<ChipComponent>().Exc<ChipViewRefComponent>().End();
      
      _chipsPool = _world.GetPool<ChipComponent>();
      _gridPositionPool = _world.GetPool<GridPositionComponent>();
      
      _chipViewRefPool = _world.GetPool<ChipViewRefComponent>();
      
      Debug.Log($"Init: {GetType().Name}");
    }

    public void Run(IEcsSystems systems)
    {    
      if (_filterChips.GetEntitiesCount() == 0)
        return;
      
      foreach (int chipEntity in _filterChips)
      {
        ref ChipViewRefComponent chipViewRef = ref _chipViewRefPool.Add(chipEntity);
        ref GridPositionComponent gridPosition = ref _gridPositionPool.Get(chipEntity);
        ref ChipComponent chip = ref _chipsPool.Get(chipEntity);
        
        chip.EntityIndex = chipEntity;
        
        var chipView = Object.Instantiate(_chipViewPrefab, _chipsParent).GetComponent<ChipView>();
        
        chipView.Construct(chipEntity, _fieldData.Offset);
        chipView.SetPosition(gridPosition.Position);
        chipView.SetType(chip.Type);
        
        chipViewRef.ChipView = chipView;
      }
      
      Debug.Log($"Run: {GetType().Name}");
    }
  }
}