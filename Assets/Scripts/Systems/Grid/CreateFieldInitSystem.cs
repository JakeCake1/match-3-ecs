using Components.Cell;
using Components.Common;
using Components.Field;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Grid
{
  public sealed class CreateFieldInitSystem : IEcsInitSystem
  {
    private readonly FieldData _fieldData;
    
    private EcsPool<CellFieldComponent> _fieldsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;

    public CreateFieldInitSystem(FieldData fieldData) =>
      _fieldData = fieldData;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();

      _fieldsPool = world.GetPool<CellFieldComponent>();
      _gridPositionsPool = world.GetPool<GridPositionComponent>();

      CreateField(world);
    }

    private void CreateField(EcsWorld world)
    {
      int fieldHeight = _fieldData.Size.y;
      int fieldWidth = _fieldData.Size.x;

      CellFieldComponent cellField = CreateFieldEntity(world, fieldWidth, fieldHeight);
      CreateFieldCells(fieldHeight, fieldWidth, world, cellField);
    }

    private CellFieldComponent CreateFieldEntity(EcsWorld world, int fieldWidth, int fieldHeight)
    {
      var gridEntity = world.NewEntity();
      ref CellFieldComponent cellField = ref _fieldsPool.Add(gridEntity);
      
      cellField.Grid = new CellComponent[fieldWidth, fieldHeight];
      
      return cellField;
    }

    private void CreateFieldCells(int fieldHeight, int fieldWidth, EcsWorld world, CellFieldComponent cellField)
    {
      for (int y = 0; y < fieldHeight; y++)
      for (int x = 0; x < fieldWidth; x++) 
          cellField.Grid[x, y] = CreateCellEntity(world, x, y);
    }

    private CellComponent CreateCellEntity(EcsWorld world, int x, int y)
    {
      int cellEntity = world.NewEntity();
      ref CellComponent cellComponent = ref world.GetPool<CellComponent>().Add(cellEntity);

      ref GridPositionComponent cellGridPosition = ref _gridPositionsPool.Add(cellEntity);

      cellGridPosition.Position = new Vector2Int(x, y);
      cellComponent.EntityIndex = cellEntity;
      
      return cellComponent;
    }
  }
}