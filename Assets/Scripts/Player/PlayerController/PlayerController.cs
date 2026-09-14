using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] private BoxCollider floorDetection;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private PlayerHorizontalMovement playerHorizontalMovement;
    [SerializeField] private PlayerJump playerJump;
    [SerializeField] private PlayerPickUp playerPickUp;
    [SerializeField] private PlayerDrop playerDrop;
    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;

    private const float epsilon = 1e-06f;

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
        moveAction = playerInput.actions["Move"];

        IdleState idleState = new IdleState(this);
        idleState.actions = new List<MonoBehaviour>()
        {
            playerHorizontalMovement,
            playerJump,
            playerPickUp,
            playerDrop
        };

        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)idleState.OnPickUp);
        eventBus.Subscribe<OnPlayerDrop>((Action)idleState.OnDrop);

        MoveState moveState = new MoveState(this);
        moveState.actions = new List<MonoBehaviour>()
        {
        playerHorizontalMovement,
        playerJump
        };

        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)moveState.OnPickUp);
        eventBus.Subscribe<OnPlayerDrop>((Action)moveState.OnDrop);

        OnAirState onAirState = new OnAirState(this);
        onAirState.actions = new List<MonoBehaviour>()
        {
            playerHorizontalMovement
        };

        OnActionLock onActionLockState = new OnActionLock(this);
        onActionLockState.actions = new List<MonoBehaviour>()
        {

        };

        eventBus.Subscribe<OnPlayerDropToIdleAnimFinished>((Action)onActionLockState.OnActionEnded);
        eventBus.Subscribe<OnPlayerPickUpAnimFinished>((Action)onActionLockState.OnActionEnded);

        Dictionary<Type, State> states = new Dictionary<Type, State>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(MoveState)] = moveState,
            [typeof(OnAirState)] = onAirState,
            [typeof(OnActionLock)] = onActionLockState
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
        private PlayerController playerController;
        private float horizontalInput = 0f;

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

            horizontalInput = playerController.moveAction.ReadValue<Vector2>().x;

            if (horizontalInput * horizontalInput > epsilon * epsilon)
            {
                playerController.fsm.TryChange<IdleState>(typeof(MoveState));
            }
        }

        public override void Exit()
        {

        }

        public void OnPickUp(OnPlayerPickUp data)
        {
            playerController.fsm.TryChange<IdleState>(typeof(OnActionLock));
        }

        public void OnDrop()
        {
            playerController.fsm.TryChange<IdleState>(typeof(OnActionLock));
        }
    }

    private class MoveState : State
    {
        private PlayerController playerController;
        private float horizontalInput = 0f;

        public MoveState(PlayerController playerController)
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
                playerController.fsm.TryChange<MoveState>(typeof(OnAirState));
            }

            horizontalInput = playerController.moveAction.ReadValue<Vector2>().x;

            if (horizontalInput * horizontalInput < epsilon * epsilon)
            {
                playerController.fsm.TryChange<MoveState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {

        }

        public void OnPickUp(OnPlayerPickUp data)
        {
            playerController.fsm.TryChange<MoveState>(typeof(OnActionLock));
        }

        public void OnDrop()
        {
            playerController.fsm.TryChange<MoveState>(typeof(OnActionLock));
        }
    }

    private class OnAirState : State
    {
        private PlayerController playerController;

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

    private class OnActionLock : State
    {
        private PlayerController playerController;

        public OnActionLock(PlayerController playerController)
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
                playerController.fsm.TryChange<OnActionLock>(typeof(OnAirState));
            }
        }

        public override void Exit()
        {

        }

        public void OnActionEnded()
        {
            playerController.fsm.TryChange<OnActionLock>(typeof(IdleState));
        }
    }
}
