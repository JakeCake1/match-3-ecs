using Gameplay.Services.AnimationService;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameServicesInstaller: IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      builder.Register<IAnimationService, AnimationService>(Lifetime.Scoped);
    }
  }
}