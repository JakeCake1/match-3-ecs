using Components.Cell;
using Components.Cell.Markers;
using Components.Chips;
using Components.Common;
using Components.Field;
using Components.Injector;
using Components.Injector.Markers;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Chips
{
  public class CreateChipsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private readonly FieldData _fieldData;

    private EcsWorld _world;

    private EcsFilter _readyInjectorsFilter;
    private EcsFilter _notBusyCellsFilter;

    private EcsPool<ChipComponent> _chipsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;
    private EcsPool<ReadyInjectorComponent> _readyInjectorsPool;
    private EcsPool<ChipsFieldComponent> _chipsFieldPool;

    private int _chipsFieldEntityIndex;

    public CreateChipsSystem(FieldData fieldData) =>
      _fieldData = fieldData;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _readyInjectorsFilter = _world.Filter<ChipsInjectorComponent>().Inc<ReadyInjectorComponent>().End();
      _notBusyCellsFilter = _world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<BusyCellComponent>().End();

      _chipsPool = _world.GetPool<ChipComponent>();
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
      _readyInjectorsPool = _world.GetPool<ReadyInjectorComponent>();
      _chipsFieldPool = _world.GetPool<ChipsFieldComponent>();

      CreateChipsField();
    }

    public void Run(IEcsSystems systems)
    {
      if (AllCellsAreBusy())
        return;
      
      foreach (int injectorEntityIndex in _readyInjectorsFilter)
      {
        CreateChip(injectorEntityIndex);
        _readyInjectorsPool.Del(injectorEntityIndex);
      }
      
      bool AllCellsAreBusy() =>
        _notBusyCellsFilter.GetEntitiesCount() == 0;
    }

    private void CreateChipsField()
    {
      _chipsFieldEntityIndex = _world.NewEntity();
      
      ref ChipsFieldComponent chipsFieldComponent = ref _chipsFieldPool.Add(_chipsFieldEntityIndex);
      chipsFieldComponent.Grid = new int[_fieldData.Size.x, _fieldData.Size.y];
            
      for (int y = 0; y < chipsFieldComponent.Grid.GetLength(1); y++)
      for (int x = 0; x < chipsFieldComponent.Grid.GetLength(0); x++) 
          chipsFieldComponent.Grid[x, y] = -1;
    }

    private void CreateChip(int injectorEntityIndex)
    {
      int chipEntityIndex = _world.NewEntity();
      
      AddChipComponent();
      ref GridPositionComponent chipPosition = ref AddGridPositionComponent();
      AttachChipToField(ref chipPosition);
      
      void AddChipComponent()
      {
        ref ChipComponent chipComponent = ref _chipsPool.Add(chipEntityIndex);
     
        chipComponent.EntityIndex = chipEntityIndex;
        chipComponent.Type = Random.Range(0, _fieldData.ChipsCount);
      }
      
      ref GridPositionComponent AddGridPositionComponent()
      {
        ref GridPositionComponent gridPositionComponent = ref _gridPositionsPool.Add(chipEntityIndex);
        gridPositionComponent.Position = _gridPositionsPool.Get(injectorEntityIndex).Position + Vector2Int.down;
        
        return ref gridPositionComponent;
      }

      void AttachChipToField(ref GridPositionComponent chipPosition)
      {
        ref ChipsFieldComponent chipsFieldComponent = ref _chipsFieldPool.Get(_chipsFieldEntityIndex);
        chipsFieldComponent.Grid[chipPosition.Position.x, chipPosition.Position.y] = chipEntityIndex;
      }
    }
  }
}