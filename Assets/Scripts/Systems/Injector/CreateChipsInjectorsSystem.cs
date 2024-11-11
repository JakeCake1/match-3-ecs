using Components.Chips;
using Components.Common;
using Components.Injector;
using Components.Injector.Markers;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Injector
{
  public class CreateChipsInjectorsSystem : IEcsInitSystem
  {
    private readonly ChipInjectorsData _chipInjectorsData;

    public CreateChipsInjectorsSystem(ChipInjectorsData chipInjectorsData) =>
      _chipInjectorsData = chipInjectorsData;

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();
      
      EcsPool<GridPosition> gridPositionPool = world.GetPool<GridPosition>();

      foreach (Vector2Int chipsInjectorsPosition in _chipInjectorsData.ChipsInjectorsPositions)
      {
        var chipInjectorEntityIndex = world.NewEntity();
        world.GetPool<ChipsInjector>().Add(chipInjectorEntityIndex);

        ref GridPosition cellGridPosition = ref gridPositionPool.Add(chipInjectorEntityIndex);

        cellGridPosition.Position = new Vector2Int(chipsInjectorsPosition.x, chipsInjectorsPosition.y);

        world.GetPool<ReadyInjector>().Add(chipInjectorEntityIndex);
      }

      Debug.Log("Init: CreateChipsInjectorsSystem");
    }
  }
}