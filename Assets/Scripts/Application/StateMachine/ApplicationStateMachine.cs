using System;
using System.Collections.Generic;
using Application.StateMachine.Interfaces;
using UnityEngine;
using VContainer;

namespace Application.StateMachine
{    
  /// \class ApplicationStateMachine
  /// \brief Машина состояний, отвечающая за переключение между состояниями приложения
  public sealed class ApplicationStateMachine : IApplicationStateMachine
  {
    /// \brief Коллекция доступных состояний
    private readonly Dictionary<Type, IApplicationExitableState> _states;
    /// \brief Контейнер зависимостей (VContainer)
    private readonly IObjectResolver _container;
    
    /// \brief Текущее состояние
    private IApplicationExitableState _activeApplicationExitableState;
    
    /// \brief Конструктор машины состояний
    /// \param container   Контейнер зависимостей (VContainer)
    public ApplicationStateMachine(IObjectResolver container)
    {
      _container = container;
      _states = new Dictionary<Type, IApplicationExitableState>();
    }
    
    /// \brief Метод входа в состояние, принимающий тип состояния как параметр
    public void Enter<TState>() where TState : class, IApplicationState
    {
      LazyCreateState<TState>();

      if (IsApplicationAlreadyInThisState<TState>()) 
        return;

      IApplicationState exitableApplicationState = ChangeState<TState>();
      exitableApplicationState.Enter();
    }
    
    /// \brief Метод создания состояния, срабатывающий при первом входе в состояние, принимающий тип состояния как параметр
    private void LazyCreateState<TState>() where TState : class, IApplicationState
    {
      if (!_states.ContainsKey(typeof(TState)))
      {
        TState state = _container.Resolve<TState>();
        _states.Add(typeof(TState), state);
      }
    }

    /// \brief Метод проверки текущего состояния, принимающий тип состояния как параметр. Нужен для того, чтобы избежать повторного входа в одно и то же состояние
    private bool IsApplicationAlreadyInThisState<TState>() where TState : class, IApplicationState
    {
      if (_activeApplicationExitableState != null && _activeApplicationExitableState.GetType() == typeof(TState))
      {
        Debug.LogWarning($"Application is already in this state {typeof(TState).Name}");
        return true;
      }

      return false;
    }
    
    /// \brief Метод изменения состояния приложения, принимающий тип состояния как параметр
    private TState ChangeState<TState>() where TState : class, IApplicationExitableState
    {
      _activeApplicationExitableState?.Exit();
      
      TState state = GetState<TState>();
      _activeApplicationExitableState = state;
      return state;
    }
    
    /// \brief Метод получения экземпляра состояния приложения, принимающий тип состояния как параметр
    private TState GetState<TState>() where TState : class, IApplicationExitableState =>
      _states[typeof(TState)] as TState;
  }
}