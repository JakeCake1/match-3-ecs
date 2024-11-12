using Components.Chips;

namespace Components.Command
{
  public struct SwapCombinationComponent
  {
    public (ChipComponent, ChipComponent) Pair;
    public bool IsUserInitiated;
  }
}