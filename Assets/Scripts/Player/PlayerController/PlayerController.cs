using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{
    private EventBus eventBus;
    private FSM fsm;

    private PlayerHorizontalMove playerHorizontalMove;

    private void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        playerHorizontalMove = GetComponent<PlayerHorizontalMove>();
    }

    private void Start()
    {
        IdleState idleState = new IdleState();
        idleState.actions = new List<MonoBehaviour>()
        {
            playerHorizontalMove
        };

        OnAirState onAirState = new OnAirState();
        onAirState.actions = new List<MonoBehaviour>()
        {
            playerHorizontalMove
        };

        Dictionary<Type, IState> states = new Dictionary<Type, IState>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(OnAirState)] = onAirState
        };

        fsm = new FSM(states);

        fsm.SetInitialState(typeof(IdleState));
    }

    private void Update()
    {
        fsm.Update();
    }

    private void OnDestroy()
    {

    }

    private class IdleState : IState
    {
        public override void Enter()
        {

        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }
    }

    private class OnAirState : IState
    {
        public override void Enter()
        {

        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }
    }
}
