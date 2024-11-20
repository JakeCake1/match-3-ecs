using Gameplay.Components.Common;
using Gameplay.Components.Injector;
using Gameplay.Components.Injector.Markers;
using Gameplay.Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Systems.Injector
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
        CreateChipInjector(world, chipsInjectorsPosition);
    }

    private void CreateChipInjector(EcsWorld world, Vector2Int chipsInjectorsPosition)
    {
      var chipInjectorEntityIndex = world.NewEntity();
        
      _chipsInjectorsPool.Add(chipInjectorEntityIndex);

      AddGridPositionComponent();

      _readyInjectorsPool.Add(chipInjectorEntityIndex);

      void AddGridPositionComponent()
      {
        ref GridPositionComponent cellGridPosition = ref _gridPositionsPool.Add(chipInjectorEntityIndex);
        cellGridPosition.Position = new Vector2Int(chipsInjectorsPosition.x, chipsInjectorsPosition.y);
      }
    }
  }
}