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
    
    private EcsPool<FieldComponent> _fieldsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;

    public CreateFieldInitSystem(FieldData fieldData) =>
      _fieldData = fieldData;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();

      _fieldsPool = world.GetPool<FieldComponent>();
      _gridPositionsPool = world.GetPool<GridPositionComponent>();

      CreateField(world);
    }

    private void CreateField(EcsWorld world)
    {
      int fieldHeight = _fieldData.Size.y;
      int fieldWidth = _fieldData.Size.x;

      FieldComponent field = CreateFieldEntity(world, fieldWidth, fieldHeight);
      CreateFieldCells(fieldHeight, fieldWidth, world, field);
    }

    private FieldComponent CreateFieldEntity(EcsWorld world, int fieldWidth, int fieldHeight)
    {
      var gridEntity = world.NewEntity();
      ref FieldComponent field = ref _fieldsPool.Add(gridEntity);
      
      field.Grid = new GridPositionComponent[fieldWidth, fieldHeight];
      
      return field;
    }

    private void CreateFieldCells(int fieldHeight, int fieldWidth, EcsWorld world, FieldComponent field)
    {
      for (int y = 0; y < fieldHeight; y++)
      for (int x = 0; x < fieldWidth; x++) 
          field.Grid[x, y] = CreateCellEntity(world, x, y);
    }

    private GridPositionComponent CreateCellEntity(EcsWorld world, int x, int y)
    {
      int cellEntity = world.NewEntity();
      world.GetPool<CellComponent>().Add(cellEntity);

      ref GridPositionComponent cellGridPosition = ref _gridPositionsPool.Add(cellEntity);

      cellGridPosition.Position = new Vector2Int(x, y);
      cellGridPosition.EntityIndex = cellEntity;
      
      return cellGridPosition;
    }
  }
}