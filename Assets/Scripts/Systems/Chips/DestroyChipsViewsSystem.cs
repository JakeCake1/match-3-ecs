using Components.Chips.Markers;
using Leopotam.EcsLite;

namespace Systems.Chips
{
  public class DestroyChipsViewsSystem : IEcsInitSystem, IEcsRunSystem
  {
    private EcsWorld _world;
    
    private EcsFilter _chipsViewsForDestroyFilter;
    private EcsPool<ChipViewForDestroyComponent> _chipsViewsForDestroyPool;

    public void Init(IEcsSystems systems)
    {
      _world = systems.GetWorld();
      
      _chipsViewsForDestroyFilter = _world.Filter<ChipViewForDestroyComponent>().End();
      _chipsViewsForDestroyPool = _world.GetPool<ChipViewForDestroyComponent>();
    }

    public void Run(IEcsSystems systems)
    {
      foreach (int commandEntityIndex in _chipsViewsForDestroyFilter)
      {
        DestroyChipView(commandEntityIndex);
        DestroyChipViewDeleteCommand(commandEntityIndex);
      }
    }

    private void DestroyChipView(int commandEntityIndex)
    {
      ref ChipViewForDestroyComponent chipForDestroyComponent = ref _chipsViewsForDestroyPool.Get(commandEntityIndex);
      chipForDestroyComponent.ChipView.Destroy();
    }

    private void DestroyChipViewDeleteCommand(int commandEntityIndex) => 
      _world.DelEntity(commandEntityIndex);
  }
}