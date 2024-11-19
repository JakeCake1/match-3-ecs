namespace Application.StateMachine.Interfaces
{  
  /// \interface IApplicationStateMachine
  /// \brief Интерфейс, описывающий машину состояний приложения
  public interface IApplicationStateMachine
  {
    /// \brief Метод входа в состояние, принимающий параметр типа состояния
    void Enter<TState>() where TState : class, IApplicationState;
  }
}