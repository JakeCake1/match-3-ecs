using System;
using Components.Cell.Markers;
using Components.Chips;
using Components.Command;
using Components.Common;
using Components.Field;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Movement
{
  public sealed class FindSwapsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _swapCommandsFilter;

    private EcsPool<SwapCommandComponent> _swapCommandsPool;
    private EcsPool<SwapCombinationComponent> _swapCombinationsPool;
    private EcsPool<ChipComponent> _chipPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<BusyCellComponent> _busyCellsPool;

    private CellFieldComponent _cellField;

    private readonly UnityEngine.Camera _camera;

    public FindSwapsSystem(UnityEngine.Camera camera) =>
      _camera = camera;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _swapCommandsFilter = _world.Filter<SwapCommandComponent>().End();

      _swapCommandsPool = _world.GetPool<SwapCommandComponent>();
      _swapCombinationsPool = _world.GetPool<SwapCombinationComponent>();
      _chipPool = _world.GetPool<ChipComponent>();
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _busyCellsPool = _world.GetPool<BusyCellComponent>();

      var fieldPool = _world.GetPool<CellFieldComponent>();
      _cellField = fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _swapCommandsFilter)
      {
        TryToSwap(_swapCommandsPool.Get(commandEntityIndex));
        _world.DelEntity(commandEntityIndex);
      }
    }

    private void TryToSwap(SwapCommandComponent swapCommand)
    {
      if(IsSwipeDiagonal())
        return;
      
      if (FindFirstChipForSwap(swapCommand, out ChipComponent firstChip, out GridPositionComponent gridPosition) == -1) 
        return;
      
      if (FindSecondChip(gridPosition, _cellField, swapCommand, out ChipComponent secondChip) == -1) 
        return;
      
      CreateSwapCombination(firstChip, secondChip);

      bool IsSwipeDiagonal() =>
        swapCommand.Ray.Item2.sqrMagnitude > 1;
    }

    private void CreateSwapCombination(ChipComponent firstChip, ChipComponent secondChip)
    {
      int swapCombinationEntityIndex = _world.NewEntity();
      
      ref SwapCombinationComponent swapCombination = ref _swapCombinationsPool.Add(swapCombinationEntityIndex);
      
      swapCombination.Pair = (firstChip, secondChip);
      swapCombination.IsUserInitiated = true;
    }

    private int FindFirstChipForSwap(SwapCommandComponent swapCommand, out ChipComponent chip, out GridPositionComponent gridPosition) //TODO: Избавиться от рейкастов
    {
      var ray = _camera.ScreenPointToRay(swapCommand.Ray.Item1);
      RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);

      chip = default;
      chip.EntityIndex = -1;
      
      gridPosition = default;

      if (!raycastHit2D) 
        return chip.EntityIndex;
      
      var chipView = raycastHit2D.collider.GetComponent<ChipView>();
        
      if (!chipView) 
        return chip.EntityIndex;
        
      chip = _chipPool.Get(chipView.EntityIndex);
      gridPosition = _gridPositionsPool.Get(chipView.EntityIndex);

      return chip.EntityIndex;
    }

    private int FindSecondChip(GridPositionComponent gridPosition, CellFieldComponent cellField, SwapCommandComponent swapCommand, out ChipComponent secondChip)
    {
      gridPosition.Position += swapCommand.Ray.Item2;

      secondChip = default;
      secondChip.EntityIndex = -1;
      
      try
      {
        BusyCellComponent busyCellComponent = _busyCellsPool.Get(cellField.Grid[gridPosition.Position.x, gridPosition.Position.y]);
        secondChip = _chipPool.Get(busyCellComponent.ChipEntityIndex);
      }
      catch (IndexOutOfRangeException)
      {
        Debug.Log("Ooops, reached borders");
      }

      return secondChip.EntityIndex;
    }
  }
}