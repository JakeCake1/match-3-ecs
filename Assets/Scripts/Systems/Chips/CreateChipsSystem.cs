using System.Text;
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
      
      ref ChipsFieldComponent chipsFieldComponent = ref _chipsFieldPool.Get(_chipsFieldEntityIndex);

      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("CreateChipsSystem");

      for (int y = chipsFieldComponent.Grid.GetLength(1) - 1; y >= 0; y--)
      {   
        string s = "";

        for (int x = 0; x < chipsFieldComponent.Grid.GetLength(0); x++)
        {
          s += $"({chipsFieldComponent.Grid[x,y].EntityIndex})/{chipsFieldComponent.Grid[x,y].Type} ";
        }

        stringBuilder.AppendLine(s);
      }
      
      Debug.Log(stringBuilder.ToString());
      
      bool AllCellsAreBusy() =>
        _notBusyCellsFilter.GetEntitiesCount() == 0;
    }

    private void CreateChipsField()
    {
      _chipsFieldEntityIndex = _world.NewEntity();
      
      ref ChipsFieldComponent chipsFieldComponent = ref _chipsFieldPool.Add(_chipsFieldEntityIndex);
      chipsFieldComponent.Grid = new ChipComponent[_fieldData.Size.x, _fieldData.Size.y + 1];
    }

    private void CreateChip(int injectorEntityIndex)
    {
      int chipEntityIndex = _world.NewEntity();

      ref ChipComponent chip = ref _chipsPool.Add(chipEntityIndex);
      ref GridPositionComponent chipPosition = ref _gridPositionsPool.Add(chipEntityIndex);
      
      chip.EntityIndex = chipEntityIndex;
      chip.Type = Random.Range(0, _fieldData.ChipsCount);
      chipPosition.Position = _gridPositionsPool.Get(injectorEntityIndex).Position;

      ref ChipsFieldComponent chipsFieldComponent = ref _chipsFieldPool.Get(_chipsFieldEntityIndex);
      chipsFieldComponent.Grid[chipPosition.Position.x, chipPosition.Position.y] = chip;
    }
  }
}