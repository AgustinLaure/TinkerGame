using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private IState currentState;

    private Dictionary<Type, IState> statesDictionary = new Dictionary<Type, IState>();

    public FSM(Dictionary<Type, IState> states, List<MonoBehaviour> actions)
    {
        statesDictionary = states;
    }

    public FSM(Dictionary<Type, IState> states)
    {
        statesDictionary = states;
    }

    public void Update()
    {
        currentState.Update();
    }

    public void SetInitialState(Type initialState)
    {
        statesDictionary.TryGetValue(initialState, out currentState);
        currentState?.Enter();

        SetActionsState(currentState.actions, true);

    }
    public void TryChange<T>(Type toState) where T : IState
    {
        if (currentState is T)
        {
            currentState?.Exit();

            SetActionsState(currentState.actions, false);

            statesDictionary.TryGetValue(toState, out currentState);

            SetActionsState(currentState.actions, true);

            currentState?.Enter();
        }
    }

    private void SetActionsState(List<MonoBehaviour> actions, bool state)
    {
        if (actions != null)
        {
            foreach (MonoBehaviour action in actions)
            {
                action.enabled = state;
            }
        }
    }
}
