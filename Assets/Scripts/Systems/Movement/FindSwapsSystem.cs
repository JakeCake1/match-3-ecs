using System;
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
    private EcsFilter _chipViewsRefsFilter;

    private EcsPool<SwapCommandComponent> _swapCommandsPool;
    private EcsPool<SwapCombinationComponent> _swapCombinationsPool;
    private EcsPool<ChipViewRefComponent> _chipViewsRefsPool;
    
    private EcsPool<ChipsFieldComponent> _chipsFieldPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;

    private readonly UnityEngine.Camera _camera;

    public FindSwapsSystem(UnityEngine.Camera camera) =>
      _camera = camera;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _swapCommandsFilter = _world.Filter<SwapCommandComponent>().End();
      _chipViewsRefsFilter = _world.Filter<ChipViewRefComponent>().End();

      _swapCommandsPool = _world.GetPool<SwapCommandComponent>();
      _swapCombinationsPool = _world.GetPool<SwapCombinationComponent>();
      _chipViewsRefsPool = _world.GetPool<ChipViewRefComponent>();
      
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();
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
        Vector2 closestPosition = Vector2.positiveInfinity;

        foreach (int chipEntityIndex in _chipViewsRefsFilter)
        {
          ref ChipViewRefComponent chipViewRefComponent = ref _chipViewsRefsPool.Get(chipEntityIndex);
          FindClosestChip(chipEntityIndex, chipViewRefComponent.ChipView, fingerDownPosition, ref firstChipEntityIndex, ref closestPosition);
        }
      }

      void FindSecondChip(out int secondChipEntityIndex, ref int firstChipIndex, ref SwapCommandComponent swapCommand)
      {
        secondChipEntityIndex = -1;

        if (firstChipIndex == -1)
          return;
        
        ref GridPositionComponent gridPosition = ref _gridPositionsPool.Get(firstChipIndex);
        Vector2Int neighbourPosition = gridPosition.Position + swapCommand.Ray.Item2;

        try
        {
          secondChipEntityIndex = _chipsFieldPool.GetRawDenseItems()[1].Grid[neighbourPosition.x, neighbourPosition.y];
        }
        catch (IndexOutOfRangeException)
        {
          Debug.Log("Ooops, reached borders");
        }
      }

      bool IsBothChipsFound(ref int chipEntityIndex1, ref int chipEntityIndex2) => 
        chipEntityIndex1 != -1 && chipEntityIndex2 != -1;
    }
    
    private void FindClosestChip(int chipEntityIndex, ChipView chipView, Vector2 fingerDownPosition, ref int targetChipEntityIndex,
      ref Vector2 targetClosestPosition)
    {
      Vector2 position = chipView.GetPosition();
     
      if(IsTouchOutOfChipBorders())
        return;
      
      if (ViewIsMoreFarThanPreviousView(ref targetClosestPosition))
        return;

      targetChipEntityIndex = chipEntityIndex;
      targetClosestPosition = position;

      bool IsTouchOutOfChipBorders() => 
        (fingerDownPosition - position).magnitude > chipView.GetSize();
      
      bool ViewIsMoreFarThanPreviousView(ref Vector2 targetClosestPosition) => 
        (fingerDownPosition - position).sqrMagnitude >= (fingerDownPosition - targetClosestPosition).sqrMagnitude;
    }
  }
}