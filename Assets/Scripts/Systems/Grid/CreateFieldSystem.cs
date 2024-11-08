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

      Field field = CreateFiledEntity(world, fieldWidth, fieldHeight);

      for (int y = 0; y < fieldHeight; y++)
      {
        for (int x = 0; x < fieldWidth; x++)
        {
          GridPosition cellGridPosition = CreateCellEntity(world, x, y);
          field.Grid[x, y] = cellGridPosition;
        }
      }

      Debug.Log("Init: CreateFieldSystem");
    }

    private GridPosition CreateCellEntity(EcsWorld world, int x, int y)
    {
      int cellEntity = world.NewEntity();
      world.GetPool<Cell>().Add(cellEntity);

      EcsPool<GridPosition> gridPositionPool = world.GetPool<GridPosition>();
      ref GridPosition cellGridPosition = ref gridPositionPool.Add(cellEntity);

      cellGridPosition.Position = new Vector2Int(x, y);
      cellGridPosition.EntityIndex = cellEntity;
      
      return cellGridPosition;
    }

    private Field CreateFiledEntity(EcsWorld world, int fieldWidth, int fieldHeight)
    {
      var gridEntity = world.NewEntity();
      ref Field field = ref world.GetPool<Field>().Add(gridEntity);
      field.Grid = new GridPosition[fieldWidth, fieldHeight];
      
      return field;
    }
  }
}