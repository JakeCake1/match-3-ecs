using System;
using Components.Cell;
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
  public class FindSwapsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _ecsFilter;

    private EcsPool<SwapCommand> _swapCommandPool;

    private EcsPool<Chip> _chipPool;
    private EcsPool<GridPosition> _gridPositionPool;
    private EcsPool<BusyCell> _busyCellPool;

    private Field _field;

    private readonly UnityEngine.Camera _camera;

    public FindSwapsSystem(UnityEngine.Camera camera) =>
      _camera = camera;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _ecsFilter = _world.Filter<SwapCommand>().End();

      _swapCommandPool = _world.GetPool<SwapCommand>();

      _chipPool = _world.GetPool<Chip>();
      _gridPositionPool = _world.GetPool<GridPosition>();

      _busyCellPool = _world.GetPool<BusyCell>();

      var fieldPool = _world.GetPool<Field>();
      _field = fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _ecsFilter)
      {
        ref SwapCommand swapCommand = ref _swapCommandPool.Get(commandEntityIndex);

        TryToSwap(swapCommand);

        _world.DelEntity(commandEntityIndex);
      }
    }

    private void TryToSwap(SwapCommand swapCommand)
    {
      bool firstChipFound = FindFirstChipForSwap(swapCommand, out Chip firstChip, out GridPosition gridPosition);
      
      if (!firstChipFound) 
        return;
      
      bool secondChipFound = FindSecondChip(gridPosition, _field, swapCommand, out Chip secondChip);

      if (!secondChipFound) 
        return;
      
      CreateSwapCombination(firstChip, secondChip);
    }

    private void CreateSwapCombination(Chip firstChip, Chip secondChip)
    {
      int swapCombinationEntity = _world.NewEntity();
      
      ref SwapCombination swapCombination = ref _world.GetPool<SwapCombination>().Add(swapCombinationEntity);
      
      swapCombination.Pair = (firstChip, secondChip);
      swapCombination.IsUserInitiated = true;
    }

    private bool FindFirstChipForSwap(SwapCommand swapCommand, out Chip chip, out GridPosition gridPosition)
    {
      var ray = _camera.ScreenPointToRay(swapCommand.Ray.Item1);
      RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);

      chip = default;
      gridPosition = default;

      if (raycastHit2D)
      {
        var chipView = raycastHit2D.collider.GetComponent<ChipView>();
        if (chipView)
        {
          chip = _chipPool.Get(chipView.Entity);
          gridPosition = _gridPositionPool.Get(chipView.Entity);
        }
      }

      return raycastHit2D;
    }

    private bool FindSecondChip(GridPosition gridPosition, Field field, SwapCommand swapCommand, out Chip secondChip)
    {
      gridPosition.Position += swapCommand.Ray.Item2;

      secondChip = default;
      bool isFound = false;

      try
      {
        GridPosition secondElementPosition = field.Grid[gridPosition.Position.x, gridPosition.Position.y];

        BusyCell busyCell = _busyCellPool.Get(secondElementPosition.EntityIndex);
        secondChip = _chipPool.Get(busyCell.ChipEntityIndex);

        isFound = true;
      }
      catch (IndexOutOfRangeException)
      {
        Debug.Log("Ooops, reached borders");
      }

      return isFound;
    }
  }
}