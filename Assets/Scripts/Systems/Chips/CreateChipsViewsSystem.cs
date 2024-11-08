using Components.Chips;
using Components.Common;
using Data;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems.Chips
{
  public class CreateChipsViewsSystem : IEcsRunSystem
  {
    private readonly ChipView _chipViewPrefab;
    private readonly FieldData _fieldData;

    public CreateChipsViewsSystem(FieldData fieldData, ChipView chipViewPrefab)
    {
      _fieldData = fieldData;
      _chipViewPrefab = chipViewPrefab;
    }

    public void Run(IEcsSystems systems)
    {    
      EcsWorld world = systems.GetWorld();

      var filterChips = world.Filter<Chip>().Exc<ChipViewRef>().End();
      
      var chipsPool = world.GetPool<Chip>();
      var gridPositionPool = world.GetPool<GridPosition>();
      
      var chipViewRefPool = world.GetPool<ChipViewRef>();

      if (filterChips.GetEntitiesCount() == 0)
        return;
      
      foreach (int chipEntity in filterChips)
      {
        ref ChipViewRef chipViewRef = ref chipViewRefPool.Add(chipEntity);
        ref GridPosition gridPosition = ref gridPositionPool.Get(chipEntity);
        ref Chip chip = ref chipsPool.Get(chipEntity);
        
        var chipView = Object.Instantiate(_chipViewPrefab).GetComponent<ChipView>();
        
        chipView.Construct(_fieldData.Offset);
        
        chipView.SetPosition(gridPosition.Position);
        chipView.SetType(chip.Type);
        
        chipViewRef.ChipView = chipView;
      }
      
      Debug.Log("Init: CreateChipsViewsSystem");
    }
  }
}