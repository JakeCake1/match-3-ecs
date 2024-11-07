using Components;
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
      int fieldHeight = _fieldData.Size.y;
      int fieldWidth = _fieldData.Size.x;

      for (int y = 0; y < fieldHeight; y++)
      {
        for (int x = 0; x < fieldWidth; x++)
        {
          EcsWorld world = systems.GetWorld();
          
          var cellEntity = world.NewEntity();
          world.GetPool<Cell>().Add(cellEntity);

          EcsPool<GridPosition> gridPositionPool = world.GetPool<GridPosition>();
          ref GridPosition cellGridPosition = ref gridPositionPool.Add(cellEntity);

          cellGridPosition.Position = new Vector2Int(x, y);
        }
      }

      Debug.Log("Init: CreateFieldSystem");
    }
  }
}