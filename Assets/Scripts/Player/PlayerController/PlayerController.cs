using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{
    private EventBus eventBus;
    private FSM fsm;

    private PlayerHorizontalMove playerHorizontalMove;

    [Header("Collision")]
    [SerializeField] private BoxCollider floorDetection;
    [SerializeField] private LayerMask groundLayer;


    private void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        playerHorizontalMove = GetComponent<PlayerHorizontalMove>();
    }

    private void Start()
    {
        IdleState idleState = new IdleState(this);
        idleState.actions = new List<MonoBehaviour>()
        {
            playerHorizontalMove
        };

        OnAirState onAirState = new OnAirState(this);
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

    private bool GetIsOnAir()
    {
        return !Physics.CheckBox(floorDetection.bounds.center, floorDetection.bounds.extents, floorDetection.transform.rotation, groundLayer);
    }

    private void OnDestroy()
    {

    }

    private class IdleState : IState
    {
        PlayerController playerController;

        public IdleState(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {
            Debug.Log("no estoy en el aire");

            if (playerController.GetIsOnAir())
            {
                playerController.fsm.TryChange<IdleState>(typeof(OnAirState));
            }
        }

        public override void Exit()
        {

        }
    }

    private class OnAirState : IState
    {
        PlayerController playerController;

        public OnAirState(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {
            Debug.Log("estoy en el aire");

            if (!playerController.GetIsOnAir())
            {
                playerController.fsm.TryChange<OnAirState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {

        }
    }
}
