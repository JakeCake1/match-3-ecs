using Application.ContainerMediator;
using Application.StateMachine.Interfaces;
using Application.StateMachine.States;
using AssetProvider;
using VContainer;
using VContainer.Unity;

namespace Application.StateMachine.ApplicationDependenciesInstaller
{
  /// \class ApplicationLifetimeScope
  /// \brief Класс, отвечающий за инъекцию сервисов в приложении
  public sealed class ApplicationLifetimeScope : LifetimeScope
  {  
    /// \brief Метод конфигурации зависимостей сервисов
    protected override void Configure(IContainerBuilder builder)
    {
      new AssetProviderInstaller().Install(builder);
      builder.Register<IDependenciesContainer, DependenciesContainer>(Lifetime.Singleton);

      RegisterEntryPoint(builder);
    }

    /// \brief Метод регистрации точки входа и основных состояний приложения
    private void RegisterEntryPoint(IContainerBuilder builder)
    {
      builder.RegisterEntryPoint<Bootstrapper>();
      
      builder.Register<IApplicationStateMachine, ApplicationStateMachine>(Lifetime.Singleton);
      
      builder.Register<ApplicationState>(Lifetime.Singleton);
      builder.Register<StartupState>(Lifetime.Singleton);
      builder.Register<EmptyState>(Lifetime.Singleton);
    }
  }
}