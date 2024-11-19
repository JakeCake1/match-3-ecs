using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;

namespace Application.ContainerMediator
{  
  /// \interface IDependenciesContainer
  /// \brief Интерфейс для посредника, связывающего логику приложения и DI Container
  public interface IDependenciesContainer
  {
    /// \brief Создание и получение зависимостей для главного состояния приложения
    UniTask CreateApplicationStateDependencies();
    IEnumerable<IEcsSystem> ResolveSystems();
    /// \brief Вызов очистки зависимостей для главного состояния приложения
    void CleanupApplicationStateDependencies();
  }
}