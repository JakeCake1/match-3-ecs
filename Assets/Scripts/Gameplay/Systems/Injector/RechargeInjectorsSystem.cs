using Gameplay.Components.Cell;
using Gameplay.Components.Cell.Markers;
using Gameplay.Components.Common;
using Gameplay.Components.Injector;
using Gameplay.Components.Injector.Markers;
using Leopotam.EcsLite;

namespace Gameplay.Systems.Injector
{
  public sealed class RechargeInjectorsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;

    private EcsFilter _notReadyInjectorsFilter;
    private EcsFilter _freeCellsFilter;

    private EcsPool<ReadyInjectorComponent> _readyInjectorsPool;
    private EcsPool<GridPositionComponent> _gridPositionsPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();

      _notReadyInjectorsFilter = _world.Filter<ChipsInjectorComponent>().Exc<ReadyInjectorComponent>().End();
      _freeCellsFilter = _world.Filter<CellComponent>().Inc<GridPositionComponent>().Exc<BusyCellComponent>().End();

      _readyInjectorsPool = _world.GetPool<ReadyInjectorComponent>();
      _gridPositionsPool = _world.GetPool<GridPositionComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      if (AllCellsAreBusy())
        return;

      foreach (int cellEntityIndex in _freeCellsFilter)
      {
        foreach (int injectorEntityIndex in _notReadyInjectorsFilter)
        {
          if (SkipIfCellNotUnderInjector(injectorEntityIndex, cellEntityIndex))
            continue;

          if (SkipIfInjectorAndCellInDifferentColumns(injectorEntityIndex, cellEntityIndex))
            continue;

          _readyInjectorsPool.Add(injectorEntityIndex);
          break;
        }
      }

      bool AllCellsAreBusy() =>
        _freeCellsFilter.GetEntitiesCount() == 0;

      bool SkipIfCellNotUnderInjector(int injectorEntityIndex, int cellEntityIndex) => 
        _gridPositionsPool.Get(injectorEntityIndex).Position.y - 1 != _gridPositionsPool.Get(cellEntityIndex).Position.y;
      
      bool SkipIfInjectorAndCellInDifferentColumns(int injectorEntityIndex, int cellEntityIndex) => 
        _gridPositionsPool.Get(injectorEntityIndex).Position.x != _gridPositionsPool.Get(cellEntityIndex).Position.x;
    }
  }
}