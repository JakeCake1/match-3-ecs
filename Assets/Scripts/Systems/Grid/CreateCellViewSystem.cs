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
      
      var filter = world.Filter<Cell>().Inc<GridPosition>().Exc<CellViewRef>().End();
      
      var cellPool = world.GetPool<CellViewRef>();
      var positionPool = world.GetPool<GridPosition>();
      
      foreach (int entity in filter) {
        ref CellViewRef cellViewRef = ref cellPool.Add(entity);
        ref GridPosition gridPosition = ref positionPool.Get(entity);

        var cellView = Object.Instantiate(_cellViewPrefab, parentObject.transform).GetComponent<CellView>();
        cellView.transform.position = gridPosition.Position + _fieldData.Offset * gridPosition.Position;
        
        cellViewRef.CellView = cellView;
      }
      
      Debug.Log("Init: CreateCellViewSystem");
    }
  }
}