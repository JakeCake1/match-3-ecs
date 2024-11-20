using Gameplay.Components.Cell;
using Gameplay.Components.Common;
using Gameplay.Data;
using Gameplay.Views;
using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Systems.Grid
{
  public sealed class CreateCellViewSystem : IEcsInitSystem, IEcsDestroySystem
  {
    private const string GridParentName = "Grid";
    
    private readonly CellView _cellViewPrefab;
    private readonly FieldData _fieldData;
    
    private EcsFilter _cellsWithoutViewsFilter;

    private EcsPool<CellViewRefComponent> _cellViewRefsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    
    private GameObject _parentObject;

    public CreateCellViewSystem(CellView cellViewPrefab, FieldData fieldData)
    {
      _fieldData = fieldData;
      _cellViewPrefab = cellViewPrefab;
    }

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();

      _cellsWithoutViewsFilter = world.Filter<CellComponent>()
        .Inc<GridPositionComponent>()
        .Exc<CellViewRefComponent>()
        .End();

      _cellViewRefsPool = world.GetPool<CellViewRefComponent>();
      _gridPositionsPool = world.GetPool<GridPositionComponent>();

      CreateCellViews();
    }

    private void CreateCellViews()
    {
      _parentObject = new GameObject(GridParentName);

      foreach (int cellEntityIndex in _cellsWithoutViewsFilter) 
        CreateCellView(cellEntityIndex);
    }

    private void CreateCellView(int cellEntityIndex)
    {
      ref GridPositionComponent gridPosition = ref _gridPositionsPool.Get(cellEntityIndex);

      var cellView = Object.Instantiate(_cellViewPrefab, _parentObject.transform).GetComponent<CellView>();
      cellView.transform.position = gridPosition.Position + _fieldData.Offset * gridPosition.Position;
      
      AttachViewToCellViewReference();

      void AttachViewToCellViewReference()
      {
        ref CellViewRefComponent cellViewRef = ref _cellViewRefsPool.Add(cellEntityIndex);
        cellViewRef.CellView = cellView;
      }
    }

    public void Destroy(IEcsSystems systems) => 
      Object.Destroy(_parentObject);
  }
}