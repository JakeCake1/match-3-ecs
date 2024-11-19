using Application.StateMachine.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace Application.StateMachine.States
{
  /// \class EmptyState
  /// \brief Класс-состояние, используемый для тестирования переходов между состояниями
  [UsedImplicitly]
  public sealed class EmptyState : IApplicationState
  {  
    /// \brief Метод входа в состояние
    public void Enter() => 
      Debug.Log("Enter EmptyState");
    
    /// \brief Метод выхода из состояния
    public void Exit() => 
      Debug.Log("Exit EmptyState");
  }
}