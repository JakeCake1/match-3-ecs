using Components.Common;
using Components.Injector;
using Components.Injector.Markers;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Injector
{
  public sealed class CreateChipsInjectorsInitSystem : IEcsInitSystem
  {
    private readonly ChipInjectorsData _chipInjectorsData;
    
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<ChipsInjectorComponent> _chipsInjectorsPool;
    private EcsPool<ReadyInjectorComponent> _readyInjectorsPool;

    public CreateChipsInjectorsInitSystem(ChipInjectorsData chipInjectorsData) =>
      _chipInjectorsData = chipInjectorsData;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();
      
      _gridPositionsPool = world.GetPool<GridPositionComponent>();
      _chipsInjectorsPool = world.GetPool<ChipsInjectorComponent>();
      _readyInjectorsPool = world.GetPool<ReadyInjectorComponent>();

      CreateChipsInjectors(world);
    }

    private void CreateChipsInjectors(EcsWorld world)
    {
      foreach (Vector2Int chipsInjectorsPosition in _chipInjectorsData.ChipsInjectorsPositions)
      {
        var chipInjectorEntityIndex = world.NewEntity();
        _chipsInjectorsPool.Add(chipInjectorEntityIndex);

        ref GridPositionComponent cellGridPosition = ref _gridPositionsPool.Add(chipInjectorEntityIndex);

        cellGridPosition.Position = new Vector2Int(chipsInjectorsPosition.x, chipsInjectorsPosition.y);

        _readyInjectorsPool.Add(chipInjectorEntityIndex);
      }
    }
  }
}