using Gameplay.Components.Cell;
using Gameplay.Components.Common;
using Gameplay.Components.Field;
using Gameplay.Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Systems.Grid
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

      CreateFieldCells(fieldHeight, fieldWidth, world);
    }

    private void CreateFieldCells(int fieldHeight, int fieldWidth, EcsWorld world)
    {    
      ref CellFieldComponent cellField = ref CreateFieldEntity(world, fieldWidth, fieldHeight);

      for (int y = 0; y < fieldHeight; y++)
      for (int x = 0; x < fieldWidth; x++) 
          cellField.Grid[x, y] = CreateCellEntity(world, x, y);
    }

    private ref CellFieldComponent CreateFieldEntity(EcsWorld world, int fieldWidth, int fieldHeight)
    {
      var gridEntityIndex = world.NewEntity();
      
      ref CellFieldComponent cellFieldComponent = ref CreateGrid();

      return ref cellFieldComponent;

      ref CellFieldComponent CreateGrid()
      {
        ref CellFieldComponent cellFieldComponent = ref _fieldsPool.Add(gridEntityIndex);
        cellFieldComponent.Grid = new int[fieldWidth, fieldHeight];

        return ref cellFieldComponent;
      }
    }

    private int CreateCellEntity(EcsWorld world, int x, int y)
    {
      int cellEntityIndex = world.NewEntity();

      AddCellComponent();
      AddGridPositionComponent();

      return cellEntityIndex;

      void AddGridPositionComponent()
      {
        ref GridPositionComponent cellGridPosition = ref _gridPositionsPool.Add(cellEntityIndex);
        cellGridPosition.Position = new Vector2Int(x, y);
      }

      void AddCellComponent()
      {
        ref CellComponent cellComponent = ref world.GetPool<CellComponent>().Add(cellEntityIndex);
        cellComponent.EntityIndex = cellEntityIndex;
      }
    }
  }
}