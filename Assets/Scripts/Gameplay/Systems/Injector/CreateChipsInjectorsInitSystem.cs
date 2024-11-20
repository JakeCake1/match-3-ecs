using Gameplay.Components.Common;
using Gameplay.Components.Field;
using Gameplay.Components.Injector;
using Gameplay.Components.Injector.Markers;
using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Systems.Injector
{
  public sealed class CreateChipsInjectorsInitSystem : IEcsInitSystem
  {
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<ChipsInjectorComponent> _chipsInjectorsPool;
    private EcsPool<ReadyInjectorComponent> _readyInjectorsPool;
    
    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();
      
      _gridPositionsPool = world.GetPool<GridPositionComponent>();
      _chipsInjectorsPool = world.GetPool<ChipsInjectorComponent>();
      _readyInjectorsPool = world.GetPool<ReadyInjectorComponent>();
      
      ref CellFieldComponent cellField = ref world.GetPool<CellFieldComponent>().GetRawDenseItems()[1];

      CreateChipsInjectors(ref cellField, world);
    }

    private void CreateChipsInjectors(ref CellFieldComponent cellField, EcsWorld world)
    {       
      int injectorsYPosition = cellField.Grid.GetLength(1);

      for (int x = 0; x < cellField.Grid.GetLength(0); x++)
      {
        Vector2Int chipsInjectorsPosition = new Vector2Int(x, injectorsYPosition);
        CreateChipInjector(world, chipsInjectorsPosition);
      }
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