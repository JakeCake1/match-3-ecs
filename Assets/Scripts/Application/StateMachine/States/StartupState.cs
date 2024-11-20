using Application.StateMachine.Interfaces;

namespace Application.StateMachine.States
{  
  public sealed class StartupState : IApplicationState
  {
    private readonly IApplicationStateMachine _applicationStateMachine;

    public StartupState(IApplicationStateMachine applicationStateMachine) => 
      _applicationStateMachine = applicationStateMachine;

    public void Enter() => 
      _applicationStateMachine.Enter<ApplicationState>();

    public void Exit()
    { }
  }
}