using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] private BoxCollider floorDetection;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private PlayerHorizontalMovement playerHorizontalMovement;
    [SerializeField] private PlayerJump playerJump;

    private EventBus eventBus;
    private FSM fsm;

    public State GetState { get { return fsm.GetCurrentState; } }

    public bool GetIsOnAirState { get { return fsm.GetCurrentState is OnAirState; } }

    private void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    private void Start()
    {
        IdleState idleState = new IdleState(this);
        idleState.actions = new List<MonoBehaviour>()
        {
            playerHorizontalMovement,
            playerJump
        };

        OnAirState onAirState = new OnAirState(this);
        onAirState.actions = new List<MonoBehaviour>()
        {
            playerHorizontalMovement
        };

        Dictionary<Type, State> states = new Dictionary<Type, State>()
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

    private class IdleState : State
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
            if (playerController.GetIsOnAir())
            {
                playerController.fsm.TryChange<IdleState>(typeof(OnAirState));
            }
        }

        public override void Exit()
        {

        }
    }

    private class OnAirState : State
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
