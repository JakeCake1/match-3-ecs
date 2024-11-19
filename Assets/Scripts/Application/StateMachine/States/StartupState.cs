using Application.StateMachine.Interfaces;

namespace Application.StateMachine.States
{  
  /// \class StartupState
  /// \brief Класс-состояние, используемый для инициализации сервисов приложения
  public sealed class StartupState : IApplicationState
  {
    /// \brief Машина состояний, отвечающая за переключение между состояниями приложения
    private readonly IApplicationStateMachine _applicationStateMachine;
    
    /// \brief Конструктор состояния
    /// \param applicationStateMachine   Машина состояний, отвечающая за переключение между состояниями приложения
    public StartupState(IApplicationStateMachine applicationStateMachine) => 
      _applicationStateMachine = applicationStateMachine;

    /// \brief Метод входа в состояние
    public void Enter() => 
      _applicationStateMachine.Enter<ApplicationState>();

    /// \brief Метод выхода из состояния
    public void Exit()
    { }
  }
}