using Components.Cell;
using Components.Common;
using Components.Field;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Grid
{
  public class CreateFieldSystem : IEcsInitSystem
  {
    private readonly FieldData _fieldData;

    public CreateFieldSystem(FieldData fieldData) =>
      _fieldData = fieldData;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();

      int fieldHeight = _fieldData.Size.y;
      int fieldWidth = _fieldData.Size.x;

      FieldComponent field = CreateFiledEntity(world, fieldWidth, fieldHeight);

      for (int y = 0; y < fieldHeight; y++)
      {
        for (int x = 0; x < fieldWidth; x++)
        {
          GridPositionComponent cellGridPosition = CreateCellEntity(world, x, y);
          field.Grid[x, y] = cellGridPosition;
        }
      }

      Debug.Log($"Init: {GetType().Name}");
    }

    private GridPositionComponent CreateCellEntity(EcsWorld world, int x, int y)
    {
      int cellEntity = world.NewEntity();
      world.GetPool<CellComponent>().Add(cellEntity);

      EcsPool<GridPositionComponent> gridPositionPool = world.GetPool<GridPositionComponent>();
      ref GridPositionComponent cellGridPosition = ref gridPositionPool.Add(cellEntity);

      cellGridPosition.Position = new Vector2Int(x, y);
      cellGridPosition.EntityIndex = cellEntity;
      
      return cellGridPosition;
    }

    private FieldComponent CreateFiledEntity(EcsWorld world, int fieldWidth, int fieldHeight)
    {
      var gridEntity = world.NewEntity();
      ref FieldComponent field = ref world.GetPool<FieldComponent>().Add(gridEntity);
      field.Grid = new GridPositionComponent[fieldWidth, fieldHeight];
      
      return field;
    }
  }
}