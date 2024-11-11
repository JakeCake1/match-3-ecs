using Components;
using Components.Cell;
using Components.Common;
using Data;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Grid
{
  public class CreateCellViewSystem : IEcsInitSystem
  {
    private readonly CellView _cellViewPrefab;
    private readonly FieldData _fieldData;

    public CreateCellViewSystem(CellView cellViewPrefab, FieldData fieldData)
    {
      _fieldData = fieldData;
      _cellViewPrefab = cellViewPrefab;
    }

    public void Init(IEcsSystems systems)
    {
      var parentObject = new GameObject("Grid");

      EcsWorld world = systems.GetWorld();

      var filter = world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<CellViewRefComponent>().End();

      var cellPool = world.GetPool<CellViewRefComponent>();
      var positionPool = world.GetPool<GridPositionComponent>();

      foreach (int entity in filter)
      {
        ref CellViewRefComponent cellViewRef = ref cellPool.Add(entity);
        ref GridPositionComponent gridPosition = ref positionPool.Get(entity);

        var cellView = Object.Instantiate(_cellViewPrefab, parentObject.transform).GetComponent<CellView>();
        cellView.transform.position = gridPosition.Position + _fieldData.Offset * gridPosition.Position;

        cellViewRef.CellView = cellView;
      }

      Debug.Log($"Init: {GetType().Name}");
    }
  }
}