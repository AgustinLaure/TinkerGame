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

    [Header("Config")]
    [SerializeField] private float minLinearToWalk;
    [SerializeField] private float walkToIdleTime;

    private float walkBaseMultiplier = 1f;

    private EventBus eventBus;

    private FSM fsm;

    #region HashReferences

    private const string animatorStateVarName = "State";
    private const string animSpeedName = "AnimSpeed";
    private const string idleToWalkStateName = "IdleToWalk";
    private const string walkStateName = "Walk";
    private const string walkToIdleStateName = "WalkToIdle";
    private const string jumpStateName = "Jump";
    private const string fallStateName = "Fall";
    private const string landStateName = "Land";
    private const string landToIdleStateName = "LandToIdle";
    private const string landToWalkStateName = "LandToWalk";
    private const string pickUpStateName = "PickUp";
    private const string dropStateName = "Drop";
    private const string aimStateName = "Aim";
    private const string throwStateName = "Throw";
    private const string dropToIdleStateName = "DropToIdle";
    private const string throwToIdleStateName = "ThrowToIdle";

    private Dictionary<State, int> statesAnimatorHash = new Dictionary<State, int>
    {
        [State.IdleToWalk] = Animator.StringToHash(idleToWalkStateName),
        [State.Walk] = Animator.StringToHash(walkStateName),
        [State.WalkToIdle] = Animator.StringToHash(walkToIdleStateName),
        [State.Jump] = Animator.StringToHash(jumpStateName),
        [State.Fall] = Animator.StringToHash(fallStateName),
        [State.Land] = Animator.StringToHash(landStateName),
        [State.LandToIdle] = Animator.StringToHash(landToIdleStateName),
        [State.LandToWalk] = Animator.StringToHash(landToWalkStateName),
        [State.PickUp] = Animator.StringToHash(pickUpStateName),
        [State.Drop] = Animator.StringToHash(dropStateName),
        [State.DropToIdle] = Animator.StringToHash(dropToIdleStateName),
        [State.Aim] = Animator.StringToHash(aimStateName),
        [State.Throw] = Animator.StringToHash(throwStateName),
        [State.ThrowToIdle] = Animator.StringToHash(throwToIdleStateName),
    };

    private int animatorStateHash = 0;
    private int animSpeedHash = 0;

    #endregion

    private const float epsilon = 1e-06f;

    private const float fallStateChangeSpeed = 5f;

    private enum State
    {
        Idle,
        IdleToWalk,
        Walk,
        WalkToIdle,
        Jump,
        Fall,
        Land,
        LandToIdle,
        LandToWalk,
        PickUp,
        Drop,
        DropToIdle,
        Aim,
        Aiming,
        Throw,
        ThrowToIdle
    }

    private void Awake()
    {
        animatorStateHash = Animator.StringToHash(animatorStateVarName);
        animSpeedHash = Animator.StringToHash(animSpeedName);
    }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        moveAction = playerInput.actions["Move"];

        IdleState idleState = new IdleState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)idleState.OnJump);
        eventBus.Subscribe<OnPlayerMovedHorizontally>((Action<OnPlayerMovedHorizontally>)idleState.OnMove);
        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)idleState.OnPickUp);
        eventBus.Subscribe<OnPlayerDrop>((Action)idleState.OnDrop);
        eventBus.Subscribe<OnPlayerAim>((Action)idleState.OnAim);

        IdleToWalkState idleToWalkState = new IdleToWalkState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)idleToWalkState.OnJump);
        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)idleToWalkState.OnPickUp);
        eventBus.Subscribe<OnPlayerDrop>((Action)idleToWalkState.OnDrop);

        WalkState walkState = new WalkState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)walkState.OnJump);
        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)walkState.OnPickUp);
        eventBus.Subscribe<OnPlayerDrop>((Action)walkState.OnDrop);

        WalkToIdleState walkToIdleState = new WalkToIdleState(this);
        eventBus.Subscribe<OnPlayerJump>((Action)walkToIdleState.OnJump);
        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)walkToIdleState.OnPickUp);
        eventBus.Subscribe<OnPlayerDrop>((Action)walkToIdleState.OnDrop);

        JumpState jumpState = new JumpState(this);

        FallState fallState = new FallState(this);
        eventBus.Subscribe<OnPlayerDetectedLand>((Action)fallState.OnLand);

        LandState landState = new LandState(this);

        LandToIdleState landToIdleState = new LandToIdleState(this);

        LandToWalkState landToWalkState = new LandToWalkState(this);

        PickUpState pickUpState = new PickUpState(this);

        DropState dropState = new DropState(this);

        DropToIdleState dropToIdleState = new DropToIdleState(this);

        AimState aimState = new AimState(this);

        AimingState aimingState = new AimingState(this);
        eventBus.Subscribe<OnPlayerThrow>((Action)aimingState.OnThrow);

        ThrowState throwState = new ThrowState(this);

        ThrowToIdleState throwToIdleState = new ThrowToIdleState(this);

        Dictionary<Type, global::State> states = new Dictionary<Type, global::State>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(IdleToWalkState)] = idleToWalkState,
            [typeof(WalkState)] = walkState,
            [typeof(WalkToIdleState)] = walkToIdleState,
            [typeof(JumpState)] = jumpState,
            [typeof(FallState)] = fallState,
            [typeof(LandState)] = landState,
            [typeof(LandToIdleState)] = landToIdleState,
            [typeof(LandToWalkState)] = landToWalkState,
            [typeof(PickUpState)] = pickUpState,
            [typeof(DropState)] = dropState,
            [typeof(DropToIdleState)] = dropToIdleState,
            [typeof(AimState)] = aimState,
            [typeof(ThrowState)] = throwState,
            [typeof(ThrowToIdleState)] = throwToIdleState
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

    private float GetCurrentAnimatioNNormalizedTime()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private float GetCurrentAnimationLength()
    {
        return animator.GetCurrentAnimatorClipInfo(0).Length;
    }

    private void AccelerateCurrentAnimation(float duration)
    {
        float animLength = GetCurrentAnimationLength();
        float currentNormTime = GetCurrentAnimatioNNormalizedTime();

        float remainingTime = animLength - animLength * currentNormTime;

        if (remainingTime > duration)
        {
            animator.SetFloat(animSpeedHash, remainingTime * (1f / duration));
        }
    }

    private class IdleState : global::State
    {
        private PlayerAnimator playerAnimator;

        public IdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Idle);
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

        public void OnPickUp(OnPlayerPickUp data)
        {
            playerAnimator.fsm.TryChange<IdleState>(typeof(PickUpState));
        }

        public void OnDrop()
        {
            playerAnimator.fsm.TryChange<IdleState>(typeof(DropState));
        }

        public void OnAim()
        {
            playerAnimator.fsm.TryChange<IdleState>(typeof(AimState));
        }
    }

    private class IdleToWalkState : global::State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0;

        public IdleToWalkState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.IdleToWalk);
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
                if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.IdleToWalk]))
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

        public void OnPickUp(OnPlayerPickUp data)
        {
            playerAnimator.fsm.TryChange<IdleToWalkState>(typeof(PickUpState));
        }

        public void OnDrop()
        {
            playerAnimator.fsm.TryChange<IdleToWalkState>(typeof(DropState));
        }
    }

    private class WalkState : global::State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0f;
        private bool foo = false;

        public WalkState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetFloat(playerAnimator.animSpeedHash, playerAnimator.walkBaseMultiplier);
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Walk);
            foo = false;
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

            float linearVelocityX = playerAnimator.rb.linearVelocity.x;

            if (horizontalInput * horizontalInput < epsilon * epsilon && linearVelocityX * linearVelocityX < playerAnimator.minLinearToWalk)
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

        public void OnPickUp(OnPlayerPickUp data)
        {
            playerAnimator.fsm.TryChange<WalkState>(typeof(PickUpState));
        }

        public void OnDrop()
        {
            playerAnimator.fsm.TryChange<WalkState>(typeof(DropState));
        }
    }

    private class WalkToIdleState : global::State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0;

        public WalkToIdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.WalkToIdle);
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

            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.WalkToIdle]))
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

        public void OnPickUp(OnPlayerPickUp data)
        {
            playerAnimator.fsm.TryChange<WalkToIdleState>(typeof(PickUpState));
        }

        public void OnDrop()
        {
            playerAnimator.fsm.TryChange<WalkToIdleState>(typeof(DropState));
        }
    }

    private class JumpState : global::State
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
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Jump);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.Jump]))
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

    private class FallState : global::State
    {
        private PlayerAnimator playerAnimator;

        public FallState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }


        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Fall);
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

    private class LandState : global::State
    {
        private PlayerAnimator playerAnimator;
        private float horizontalInput = 0f;

        public LandState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Land);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.Land]))
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

    private class LandToIdleState : global::State
    {
        private PlayerAnimator playerAnimator;

        public LandToIdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.LandToIdle);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.LandToIdle]))
            {
                playerAnimator.fsm.TryChange<LandToIdleState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {

        }
    }

    private class LandToWalkState : global::State
    {
        private PlayerAnimator playerAnimator;

        public LandToWalkState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.LandToWalk);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.LandToWalk]))
            {
                playerAnimator.fsm.TryChange<LandToWalkState>(typeof(WalkState));
            }
        }

        public override void Exit()
        {

        }
    }

    private class PickUpState : global::State
    {
        private PlayerAnimator playerAnimator;

        public PickUpState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.PickUp);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.PickUp]))
            {
                playerAnimator.fsm.TryChange<PickUpState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {
            playerAnimator.eventBus.Raise<OnPlayerPickUpAnimFinished>();
        }
    }

    private class DropState : global::State
    {
        private PlayerAnimator playerAnimator;

        public DropState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Drop);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.Drop]))
            {
                playerAnimator.fsm.TryChange<DropState>(typeof(DropToIdleState));
            }
        }

        public override void Exit()
        {
            playerAnimator.eventBus.Raise<OnPlayerDropAnimFinished>();
        }
    }

    private class DropToIdleState : global::State
    {
        private PlayerAnimator playerAnimator;

        public DropToIdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.DropToIdle);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.DropToIdle]))
            {
                playerAnimator.fsm.TryChange<DropToIdleState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {
            
        }
    }

    private class AimState : global::State
    {
        private PlayerAnimator playerAnimator;

        public AimState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Aim);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.Aim]))
            {
                playerAnimator.fsm.TryChange<AimState>(typeof(AimingState));
            }
        }

        public override void Exit()
        {

        }
    }

    private class AimingState : global::State
    {
        private PlayerAnimator playerAnimator;

        public AimingState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Aiming);
        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }

        public void OnThrow()
        {
            playerAnimator.fsm.TryChange<AimingState>(typeof(ThrowState));
        }
    }

    private class ThrowState : global::State
    {
        private PlayerAnimator playerAnimator;

        public ThrowState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.Throw);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.Throw]))
            {
                playerAnimator.fsm.TryChange<ThrowState>(typeof(ThrowToIdleState));
            }
        }

        public override void Exit()
        {
            playerAnimator.eventBus.Raise<OnPlayerThrowAnimFinished>();
        }
    }

    private class ThrowToIdleState : global::State
    {
        private PlayerAnimator playerAnimator;

        public ThrowToIdleState(PlayerAnimator playerAnimator)
        {
            this.playerAnimator = playerAnimator;
        }

        public override void Enter()
        {
            playerAnimator.animator.SetInteger(playerAnimator.animatorStateHash, (int)State.ThrowToIdle);
        }

        public override void Update()
        {
            if (playerAnimator.GetCurrentAnimationEnded(playerAnimator.statesAnimatorHash[State.ThrowToIdle]))
            {
                playerAnimator.fsm.TryChange<ThrowToIdleState>(typeof(IdleState));
            }
        }

        public override void Exit()
        {

        }
    }
}
