using Components.Chips;
using Components.Common;
using Data;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Chips
{
  public sealed class CreateChipsViewsSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
  {
    private readonly ChipView _chipViewPrefab;
    private readonly FieldData _fieldData;

    private EcsFilter _chipsWithoutViews;

    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<ChipViewRefComponent> _chipViewRefsPool;

    private Transform _chipsParent;

    public CreateChipsViewsSystem(FieldData fieldData, ChipView chipViewPrefab)
    {
      _chipViewPrefab = chipViewPrefab;
      _fieldData = fieldData;
    }

    public void Init(IEcsSystems systems)
    {
      var world = systems.GetWorld();

      _chipsWithoutViews = world.Filter<ChipComponent>().Exc<ChipViewRefComponent>().End();

      _chipsPool = world.GetPool<ChipComponent>();
      _gridPositionsPool = world.GetPool<GridPositionComponent>();
      _chipViewRefsPool = world.GetPool<ChipViewRefComponent>();

      CreateChipsParentObject();
    }

    public void Run(IEcsSystems systems)
    {
      if (AllChipsHaveViews())
        return;

      foreach (int chipEntity in _chipsWithoutViews) 
        CreateChipView(chipEntity);

      bool AllChipsHaveViews() => 
        _chipsWithoutViews.GetEntitiesCount() == 0;
    }

    public void Destroy(IEcsSystems systems) => 
      DestroyChipsParentObject();

    private void CreateChipView(int chipEntity)
    {
      ref ChipViewRefComponent chipViewRef = ref _chipViewRefsPool.Add(chipEntity);
      ref GridPositionComponent gridPosition = ref _gridPositionsPool.Get(chipEntity);
      ref ChipComponent chip = ref _chipsPool.Get(chipEntity);

      chip.EntityIndex = chipEntity;

      var chipView = Object.Instantiate(_chipViewPrefab, _chipsParent).GetComponent<ChipView>();

      chipView.Construct(chipEntity, _fieldData.Offset);
      chipView.SetPosition(gridPosition.Position);
      chipView.SetType(chip.Type);

      chipViewRef.ChipView = chipView;
    }

    private void CreateChipsParentObject() =>
      _chipsParent = new GameObject("Chips").transform;

    private void DestroyChipsParentObject() =>
      Object.Destroy(_chipsParent);
  }
}