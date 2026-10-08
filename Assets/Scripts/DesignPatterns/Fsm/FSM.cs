using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private State currentState;

    private Dictionary<Type, State> statesDictionary = new Dictionary<Type, State>();

    public State GetCurrentState { get { return currentState; } }

    public FSM(Dictionary<Type, State> states, List<MonoBehaviour> actions)
    {
        statesDictionary = states;
    }

    public FSM(Dictionary<Type, State> states)
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
    public void TryChange<T>(Type toState) where T : State
    {
        if (currentState is T)
        {
            currentState?.Exit();

            State nextState;

            statesDictionary.TryGetValue(toState, out nextState);

            SetNonMatchingActionsState(currentState.actions, nextState.actions);

            currentState = nextState;

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

    private void SetNonMatchingActionsState(List<MonoBehaviour> prevActions, List<MonoBehaviour> nextActions)
    {
        if (prevActions != null && nextActions != null)
        {
            foreach (MonoBehaviour prevAction in prevActions)
            {
                if (!nextActions.Contains(prevAction))
                {
                    prevAction.enabled = false;
                }
            }

            foreach (MonoBehaviour nextAction in nextActions)
            {
                if (!prevActions.Contains(nextAction))
                {
                    nextAction.enabled = true;
                }
            }
        }
    }
}
