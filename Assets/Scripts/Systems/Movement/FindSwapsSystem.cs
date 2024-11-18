using Components.Cell;
using Components.Command;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Movement
{
  public sealed class FindSwapsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _swapCommandsFilter;

    private EcsPool<SwapCommandComponent> _swapCommandsPool;
    private EcsPool<SwapCombinationComponent> _swapCombinationsPool;
    private EcsPool<CellViewRefComponent> _cellViewRefsPool;

    private EcsPool<CellFieldComponent> _cellFieldPool;
    private EcsPool<ChipsFieldComponent> _chipsFieldPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;

    private readonly UnityEngine.Camera _camera;

    private Vector2 _fieldMinimalCorner;
    private Vector2 _fieldMaximumCorner;
    private Vector2Int _fieldDensityModifier;

    public FindSwapsSystem(UnityEngine.Camera camera) =>
      _camera = camera;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _swapCommandsFilter = _world.Filter<SwapCommandComponent>().End();

      _swapCommandsPool = _world.GetPool<SwapCommandComponent>();
      _swapCombinationsPool = _world.GetPool<SwapCombinationComponent>();

      _cellViewRefsPool = _world.GetPool<CellViewRefComponent>();

      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
      _cellFieldPool = _world.GetPool<CellFieldComponent>();

      PrepareFieldBorders(ref _cellFieldPool.GetRawDenseItems()[1]);
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _swapCommandsFilter)
      {
        TryToSwap(ref _swapCommandsPool.Get(commandEntityIndex));
        _world.DelEntity(commandEntityIndex);
      }
    }

    private void TryToSwap(ref SwapCommandComponent swapCommand)
    {
      if (IsSwipeDiagonal(ref swapCommand))
        return;

      if (!FindSwapChipsEntitiesIndexes(ref swapCommand, out int firstChipEntityIndex, out int secondChipEntityIndex))
        return;

      CreateSwapCombination(firstChipEntityIndex, secondChipEntityIndex);

      bool IsSwipeDiagonal(ref SwapCommandComponent swapCommand) =>
        swapCommand.Ray.Item2.sqrMagnitude > 1;
    }

    private void CreateSwapCombination(int firstChipEntityIndex, int secondChipEntityIndex)
    {
      int swapCombinationEntityIndex = _world.NewEntity();
      SetupSwapCombination();

      void SetupSwapCombination()
      {
        ref SwapCombinationComponent swapCombination = ref _swapCombinationsPool.Add(swapCombinationEntityIndex);
        swapCombination.Pair = (firstChipEntityIndex, secondChipEntityIndex);
        swapCombination.IsUserInitiated = true;
      }
    }

    private bool FindSwapChipsEntitiesIndexes(ref SwapCommandComponent swapCommand, out int chipEntityIndex1, out int chipEntityIndex2)
    {
      FindFirstChip(out chipEntityIndex1, ref swapCommand);
      FindSecondChip(out chipEntityIndex2, ref chipEntityIndex1, ref swapCommand);

      return IsBothChipsFound(ref chipEntityIndex1, ref chipEntityIndex2);

      void FindFirstChip(out int firstChipEntityIndex, ref SwapCommandComponent swapCommand)
      {
        firstChipEntityIndex = -1;

        Vector2 fingerDownPosition = _camera.ScreenToWorldPoint(swapCommand.Ray.Item1);

        Vector2 position = (fingerDownPosition - _fieldMinimalCorner) / (_fieldMaximumCorner - _fieldMinimalCorner) * _fieldDensityModifier;
        Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

        if (IsPositionInsideField(targetPosition))
          firstChipEntityIndex = _chipsFieldPool.GetRawDenseItems()[1].Grid[targetPosition.x, targetPosition.y];
      }

      void FindSecondChip(out int secondChipEntityIndex, ref int firstChipIndex, ref SwapCommandComponent swapCommand)
      {
        secondChipEntityIndex = -1;

        if (firstChipIndex == -1)
          return;

        ref GridPositionComponent gridPosition = ref _gridPositionsPool.Get(firstChipIndex);
        Vector2Int neighbourPosition = gridPosition.Position + swapCommand.Ray.Item2;

        if (IsPositionInsideField(neighbourPosition))
          secondChipEntityIndex = _chipsFieldPool.GetRawDenseItems()[1].Grid[neighbourPosition.x, neighbourPosition.y];
      }

      bool IsBothChipsFound(ref int chipEntityIndex1, ref int chipEntityIndex2) =>
        chipEntityIndex1 != -1 && chipEntityIndex2 != -1;

      bool IsPositionInsideField(Vector2Int gridPosition) =>
        gridPosition.x >= 0 && gridPosition.x <= _fieldDensityModifier.x && gridPosition.y >= 0 && gridPosition.y <= _fieldDensityModifier.y;
    }

    private void PrepareFieldBorders(ref CellFieldComponent cellsFieldComponent)
    {
      int width = cellsFieldComponent.Grid.GetLength(0);
      int height = cellsFieldComponent.Grid.GetLength(1);

      _fieldDensityModifier = new Vector2Int(width - 1, height - 1);

      int leftDownChipEntityIndex = cellsFieldComponent.Grid[0, 0];
      int upperRightChipEntityIndex = cellsFieldComponent.Grid[_fieldDensityModifier.x, _fieldDensityModifier.y];

      _fieldMinimalCorner = _cellViewRefsPool.Get(leftDownChipEntityIndex).CellView.GetPosition();
      _fieldMaximumCorner = _cellViewRefsPool.Get(upperRightChipEntityIndex).CellView.GetPosition();
    }
  }
}