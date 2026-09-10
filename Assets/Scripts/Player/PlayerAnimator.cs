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

    private int jumpAnimHash = 0;
    private readonly string jumpStateName = "Jump";

    private int fallAnimHash = 0;
    private readonly string fallStateName = "Fall";

    private int landAnimHash = 0;
    private readonly string landStateName = "Land";

    private const float epsilon = 1e-06f;

    private const float fallStateChangeSpeed = 5f;

    private void Awake()
    {
        animatorStateHash = Animator.StringToHash(animatorStateVarName);
        jumpAnimHash = Animator.StringToHash(jumpStateName);
        fallAnimHash = Animator.StringToHash(fallStateName);
        landAnimHash = Animator.StringToHash(landStateName);
    }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        moveAction = playerInput.actions["Move"];

        IdleState idleState = new IdleState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)idleState.OnJump);
        eventBus.Subscribe<OnPlayerMovedHorizontally>((Action<OnPlayerMovedHorizontally>)idleState.OnMove);

        WalkState walkState = new WalkState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)walkState.OnJump);
        eventBus.Subscribe<OnPlayerMovedHorizontally>((Action<OnPlayerMovedHorizontally>)walkState.OnMove);

        JumpState jumpState = new JumpState(this);

        FallState fallState = new FallState(this);
        eventBus.Subscribe<OnPlayerDetectedLand>((Action)fallState.OnLand);

        LandState landState = new LandState(this);

        Dictionary<Type, State> states = new Dictionary<Type, State>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(WalkState)] = walkState,
            [typeof(JumpState)] = jumpState,
            [typeof(FallState)] = fallState,
            [typeof(LandState)] = landState
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
        //return animatorInfo.normalizedTime >= 1f;
    }

    private class IdleState : State
    {
        PlayerAnimator playerAnimator;

        public IdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, 0);
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
            playerAnimator.fsm.TryChange<IdleState>(typeof(WalkState));
        }
    }

    private class WalkState : State
    {
        PlayerAnimator playerAnimator;
        float horizontalInput = 0f;

        public WalkState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, 1);
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
                playerAnimator.fsm.TryChange<WalkState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {

        }

        public void OnJump()
        {
            playerAnimator.fsm.TryChange<WalkState>(typeof(JumpState));
        }

        public void OnMove(OnPlayerMovedHorizontally data)
        {

        }
    }

    private class JumpState : State
    {
        PlayerAnimator playerAnimator;
        bool isOnAir = false;
        float horizontalInput = 0f;

        public JumpState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, 2);
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
        PlayerAnimator playerAnimator;
       //bool isOnAir = false;
       //float horizontalInput = 0f;

        public FallState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, 3);
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
        PlayerAnimator playerAnimator;
        float horizontalInput = 0f;

        public LandState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, 4);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.landAnimHash))
            {
                horizontalInput = playerAnimator.moveAction.ReadValue<Vector2>().x;

                if (horizontalInput * horizontalInput < epsilon * epsilon)
                {
                    playerAnimator.fsm.TryChange<LandState>(typeof(IdleState));
                }
                else
                {
                    playerAnimator.fsm.TryChange<LandState>(typeof(WalkState));
                }
            }
        }

        public override void Exit()
        {

        }
    }
}
