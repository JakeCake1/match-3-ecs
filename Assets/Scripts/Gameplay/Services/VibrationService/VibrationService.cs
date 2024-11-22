namespace Gameplay.Services.VibrationService
{
  public class VibrationService : IVibrationService
  {
    public void Initialize() => 
      Vibration.Init();

    public void Vibrate() => 
      Vibration.VibratePop();
  }
}