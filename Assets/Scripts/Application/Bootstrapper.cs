using Application.StateMachine.Interfaces;
using Application.StateMachine.States;
using UnityEngine;
using VContainer;

namespace Application
{
  /// \class Bootstrapper
  /// \brief Это основной класс, являющийся точкой входа в приложение
  public sealed class Bootstrapper : MonoBehaviour
  {
    /// \brief Ссылка на стейт машину, отвечающую за переключения состояний приложения
    private IApplicationStateMachine _applicationStateMachine;
   
    /// \brief Метод - аналог констурктора. В нем мы через инъекцию получаем ссылку на стейт машину
    [Inject]
    public void Construct(IApplicationStateMachine applicationStateMachine) =>
      _applicationStateMachine = applicationStateMachine;

    /// \brief В этом методе осуществялется вход в состояние запуска сервисов, необходимых для работы приложения
    private void Start() =>
      _applicationStateMachine.Enter<StartupState>();
  }
}