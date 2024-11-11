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

    private EcsPool<SwapCommandComponent> _swapCommandPool;

    private EcsPool<ChipComponent> _chipPool;
    private EcsPool<GridPositionComponent> _gridPositionPool;
    private EcsPool<BusyCellComponent> _busyCellPool;

    private FieldComponent _field;

    private readonly UnityEngine.Camera _camera;

    public FindSwapsSystem(UnityEngine.Camera camera) =>
      _camera = camera;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _ecsFilter = _world.Filter<SwapCommandComponent>().End();

      _swapCommandPool = _world.GetPool<SwapCommandComponent>();

      _chipPool = _world.GetPool<ChipComponent>();
      _gridPositionPool = _world.GetPool<GridPositionComponent>();

      _busyCellPool = _world.GetPool<BusyCellComponent>();

      var fieldPool = _world.GetPool<FieldComponent>();
      _field = fieldPool.GetRawDenseItems()[1];
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _ecsFilter)
      {
        ref SwapCommandComponent swapCommand = ref _swapCommandPool.Get(commandEntityIndex);

        TryToSwap(swapCommand);

        _world.DelEntity(commandEntityIndex);
      }
    }

    private void TryToSwap(SwapCommandComponent swapCommand)
    {
      if(swapCommand.Ray.Item2.sqrMagnitude > 1)
        return;
      
      bool firstChipFound = FindFirstChipForSwap(swapCommand, out ChipComponent firstChip, out GridPositionComponent gridPosition);
      
      if (!firstChipFound) 
        return;
      
      bool secondChipFound = FindSecondChip(gridPosition, _field, swapCommand, out ChipComponent secondChip);

      if (!secondChipFound) 
        return;
      
      CreateSwapCombination(firstChip, secondChip);
    }

    private void CreateSwapCombination(ChipComponent firstChip, ChipComponent secondChip)
    {
      int swapCombinationEntity = _world.NewEntity();
      
      ref SwapCombinationComponent swapCombination = ref _world.GetPool<SwapCombinationComponent>().Add(swapCombinationEntity);
      
      swapCombination.Pair = (firstChip, secondChip);
      swapCombination.IsUserInitiated = true;
    }

    private bool FindFirstChipForSwap(SwapCommandComponent swapCommand, out ChipComponent chip, out GridPositionComponent gridPosition)
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

    private bool FindSecondChip(GridPositionComponent gridPosition, FieldComponent field, SwapCommandComponent swapCommand, out ChipComponent secondChip)
    {
      gridPosition.Position += swapCommand.Ray.Item2;

      secondChip = default;
      bool isFound = false;

      try
      {
        GridPositionComponent secondElementPosition = field.Grid[gridPosition.Position.x, gridPosition.Position.y];

        BusyCellComponent busyCellComponent = _busyCellPool.Get(secondElementPosition.EntityIndex);
        secondChip = _chipPool.Get(busyCellComponent.ChipEntityIndex);

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