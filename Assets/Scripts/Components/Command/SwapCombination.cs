using Components.Chips;

namespace Components.Command
{
  public struct SwapCombination
  {
    public (Chip, Chip) Pair;
    public bool IsUserInitiated;
  }
}