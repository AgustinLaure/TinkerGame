using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;

    private EventBus eventBus;

    private FSM fsm;

    private int animatorStateHash = 0;
    private readonly string animatorStateVarName = "State";

    private int idleToWalkAnimHash = 0;
    private readonly string idleToWalkStateName = "IdleToWalk";

    private int walkAnimHash = 0;
    private readonly string walkStateName = "Walk";

    private int walkToIdleAnimHash = 0;
    private readonly string walkToIdleStateName = "WalkToIdle";

    private int jumpAnimHash = 0;
    private readonly string jumpStateName = "Jump";

    private int fallAnimHash = 0;
    private readonly string fallStateName = "Fall";

    private int landAnimHash = 0;
    private readonly string landStateName = "Land";

    private int landToIdleAnimHash = 0;
    private readonly string landToIdleStateName = "LandToIdle";

    private int landToWalkAnimHash = 0;
    private readonly string landToWalkStateName = "LandToWalk";

    private const float epsilon = 1e-06f;

    private const float fallStateChangeSpeed = 5f;

    private enum States
    {
        Idle,
        IdleToWalk,
        Walk,
        WalkToIdle,
        Jump,
        Fall,
        Land,
        LandToIdle,
        LandToWalk
    }

    private void Awake()
    {
        animatorStateHash = Animator.StringToHash(animatorStateVarName);
        idleToWalkAnimHash = Animator.StringToHash(idleToWalkStateName);
        walkAnimHash = Animator.StringToHash(walkStateName);
        walkToIdleAnimHash = Animator.StringToHash(walkToIdleStateName);
        jumpAnimHash = Animator.StringToHash(jumpStateName);
        fallAnimHash = Animator.StringToHash(fallStateName);
        landAnimHash = Animator.StringToHash(landStateName);
        landToIdleAnimHash = Animator.StringToHash(landToIdleStateName);
        landToWalkAnimHash = Animator.StringToHash(landToWalkStateName);
    }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        moveAction = playerInput.actions["Move"];

        IdleState idleState = new IdleState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)idleState.OnJump);
        eventBus.Subscribe<OnPlayerMovedHorizontally>((Action<OnPlayerMovedHorizontally>)idleState.OnMove);

        IdleToWalkState idleToWalkState = new IdleToWalkState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)idleToWalkState.OnJump);

        WalkState walkState = new WalkState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)walkState.OnJump);

        WalkToIdleState walkToIdleState = new WalkToIdleState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)walkToIdleState.OnJump);

        JumpState jumpState = new JumpState(this);

        FallState fallState = new FallState(this);
        eventBus.Subscribe<OnPlayerDetectedLand>((Action)fallState.OnLand);

        LandState landState = new LandState(this);

        LandToIdleState landToIdleState = new LandToIdleState(this);

        LandToWalkState landToWalkState = new LandToWalkState(this);

        Dictionary<Type, State> states = new Dictionary<Type, State>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(IdleToWalkState)] = idleToWalkState,
            [typeof(WalkState)] = walkState,
            [typeof(WalkToIdleState)] = walkToIdleState,
            [typeof(JumpState)] = jumpState,
            [typeof(FallState)] = fallState,
            [typeof(LandState)] = landState,
            [typeof(LandToIdleState)] = landToIdleState,
            [typeof(LandToWalkState)] = landToWalkState
        };

        fsm = new FSM(states);

        fsm.SetInitialState(typeof(IdleState));
    }

    private void Update()
    {
        fsm.Update();
    }

    private bool GetCurrentAnimationEnded(int currentAnimHash)
    {
        AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);

        return animatorInfo.normalizedTime >= 1f && animatorInfo.shortNameHash == currentAnimHash;
    }

    private class IdleState : State
    {
        private PlayerAnimator playerAnimator;

        public IdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.Idle);
        }

        public override void Update()
        {
            if (playerAnimator.playerController.GetIsOnAirState)
            {
                if (playerAnimator.rb.linearVelocity.y < -fallStateChangeSpeed)
                {
                    playerAnimator.fsm.TryChange<IdleState>(typeof(FallState));
                }
            }
        }

        public override void Exit()
        {

        }

        public void OnJump()
        {
            playerAnimator.fsm.TryChange<IdleState>(typeof(JumpState));
        }

        public void OnMove(OnPlayerMovedHorizontally data)
        {
            playerAnimator.fsm.TryChange<IdleState>(typeof(IdleToWalkState));
        }
    }

    private class IdleToWalkState : State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0;

        public IdleToWalkState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.IdleToWalk);
        }

        public override void Update()
        {
            horizontalInput = playerAnimator.moveAction.ReadValue<Vector2>().x;

            bool isStatic = horizontalInput * horizontalInput < epsilon * epsilon;

            if (isStatic)
            {
                playerAnimator.fsm.TryChange<IdleToWalkState>(typeof(IdleState));
            }
            else
            {
                if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.idleToWalkAnimHash))
                {
                    playerAnimator.fsm.TryChange<IdleToWalkState>(typeof(WalkState));
                }
            }

            if (playerAnimator.playerController.GetIsOnAirState)
            {
                if (playerAnimator.rb.linearVelocity.y < -fallStateChangeSpeed)
                {
                    playerAnimator.fsm.TryChange<IdleToWalkState>(typeof(FallState));
                }
            }
        }

        public override void Exit()
        {

        }

        public void OnJump()
        {
            playerAnimator.fsm.TryChange<IdleToWalkState>(typeof(JumpState));
        }
    }

    private class WalkState : State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0f;

        public WalkState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.Walk);
        }

        public override void Update()
        {
            horizontalInput = playerAnimator.moveAction.ReadValue<Vector2>().x;

            if (playerAnimator.playerController.GetIsOnAirState)
            {
                if (playerAnimator.rb.linearVelocity.y < -fallStateChangeSpeed)
                {
                    playerAnimator.fsm.TryChange<WalkState>(typeof(FallState));
                }
            }

            if (horizontalInput * horizontalInput < epsilon * epsilon)
            {
                playerAnimator.fsm.TryChange<WalkState>(typeof(WalkToIdleState));
            }
        }

        public override void Exit()
        {

        }

        public void OnJump()
        {
            playerAnimator.fsm.TryChange<WalkState>(typeof(JumpState));
        }
    }

    private class WalkToIdleState : State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0;

        public WalkToIdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.WalkToIdle);
        }

        public override void Update()
        {
            horizontalInput = playerAnimator.moveAction.ReadValue<Vector2>().x;

            if (playerAnimator.playerController.GetIsOnAirState)
            {
                if (playerAnimator.rb.linearVelocity.y < -fallStateChangeSpeed)
                {
                    playerAnimator.fsm.TryChange<WalkState>(typeof(FallState));
                }
            }

            if (horizontalInput > epsilon * epsilon)
            {
                playerAnimator.fsm.TryChange<WalkToIdleState>(typeof(WalkState));
            }

            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.walkToIdleAnimHash))
            {
                playerAnimator.fsm.TryChange<WalkToIdleState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {

        }

        public void OnJump()
        {
            playerAnimator.fsm.TryChange<WalkToIdleState>(typeof(JumpState));
        }
    }

    private class JumpState : State
    {
        private PlayerAnimator playerAnimator;
        private bool isOnAir = false;
        private float horizontalInput = 0f;

        public JumpState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.Jump);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.jumpAnimHash))
            {
                isOnAir = playerAnimator.playerController.GetIsOnAirState;
                horizontalInput = playerAnimator.moveAction.ReadValue<Vector2>().x;

                if (isOnAir)
                {
                    if (playerAnimator.rb.linearVelocity.y < 0f)
                    {
                        playerAnimator.fsm.TryChange<JumpState>(typeof(FallState));
                    }
                }

                if (horizontalInput * horizontalInput < epsilon * epsilon)
                {
                    if (!isOnAir)
                    {
                        playerAnimator.fsm.TryChange<JumpState>(typeof(LandState));
                    }
                }
            }
        }

        public override void Exit()
        {

        }
    }

    private class FallState : State
    {
        private PlayerAnimator playerAnimator;

        public FallState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }


        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.Fall);
        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }

        public void OnLand()
        {
            playerAnimator.fsm.TryChange<FallState>(typeof(LandState));
        }
    }

    private class LandState : State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0f;

        public LandState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.Land);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.landAnimHash))
            {
                horizontalInput = playerAnimator.moveAction.ReadValue<Vector2>().x;

                if (horizontalInput * horizontalInput > epsilon * epsilon)
                {
                    playerAnimator.fsm.TryChange<LandState>(typeof(LandToWalkState));
                }
                else
                {
                    playerAnimator.fsm.TryChange<LandState>(typeof(LandToIdleState));
                }
            }
        }

        public override void Exit()
        {

        }
    }

    private class LandToIdleState : State
    {
        private PlayerAnimator playerAnimator;

        public LandToIdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.LandToIdle);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.landToIdleAnimHash))
            {
                playerAnimator.fsm.TryChange<LandToIdleState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {

        }
    }

    private class LandToWalkState : State
    {
        private PlayerAnimator playerAnimator;

        public LandToWalkState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)States.LandToWalk);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.landToWalkAnimHash))
            {
                playerAnimator.fsm.TryChange<LandToWalkState>(typeof(WalkState));
            }
        }

        public override void Exit()
        {

        }
    }
}
