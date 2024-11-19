using System.Collections.Generic;
using Application.StateMachine.ApplicationDependenciesInstaller;
using AssetProvider.Scripts;
using Cysharp.Threading.Tasks;
using Gameplay;
using Leopotam.EcsLite;
using VContainer;
using VContainer.Unity;

namespace Application.ContainerMediator
{
  /// \class DependenciesContainer
  /// \brief Класс-посредник, связывающий логику приложения и DI Container
  public class DependenciesContainer : IDependenciesContainer
  {
    /// \brief Объект - глобальная область существования, содержащий регистрацию инъекций сервисов для всего времени жизни приложения
    private readonly ApplicationLifetimeScope _applicationLifetimeScope;

    private readonly IAssetProvider _assetProvider;
    
    private LifetimeScope _applicationScope;
    private GamePlayInstaller _gamePlayInstaller;

    /// \brief Конструктор посредника контейнера
    /// \param applicationLifetimeScope   Объект - глобальная область существования, содержащий регистрацию инъекций сервисов для всего времени жизни приложения
    public DependenciesContainer(ApplicationLifetimeScope applicationLifetimeScope, IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
      _applicationLifetimeScope = applicationLifetimeScope;
    }

    /// \brief Создание и получение зависимостей для главного состояния приложения
    public async UniTask CreateApplicationStateDependencies()
    {       
      _gamePlayInstaller = new GamePlayInstaller(_assetProvider);
      var gameSystemsInstaller = new GameSystemsInstaller();
      
      await _gamePlayInstaller.Preload();
      
      _applicationScope = _applicationLifetimeScope.CreateChild(builder =>
      {
        _gamePlayInstaller.Install(builder);
        gameSystemsInstaller.Install(builder);
      });
    }

    public IEnumerable<IEcsSystem> ResolveSystems() => 
      _applicationScope.Container.Resolve<IEnumerable<IEcsSystem>>();

    /// \brief Вызов очистки зависимостей для главного состояния приложения
    public void CleanupApplicationStateDependencies()
    {
      _gamePlayInstaller.Clear();
      
      _applicationScope.Dispose();
      _applicationScope = null;
    }
  }
}