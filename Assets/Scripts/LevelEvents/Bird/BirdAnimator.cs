using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BirdAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    private enum State
    {
        Idle,
    }

    #region HashReferences

    private const string animatorStateVarName = "State";

    private const string idleStateName = "Idle";

    private Dictionary<State, int> statesAnimatorHash = new Dictionary<State, int>
    {
        [State.Idle] = Animator.StringToHash(idleStateName),
    };

    #endregion

    private EventBus eventBus;
    private FSM fsm;

    private int animatorStateHash = 0;

    private void Awake()
    {
        animatorStateHash = Animator.StringToHash(animatorStateVarName);
    }

    private void Start()
    {
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        eventBus = serviceLocator.GetService<EventBus>();

        IdleState idleState = new IdleState(this);

        Dictionary<Type, global::State> states = new Dictionary<Type, global::State>()
        {
            [typeof(IdleState)] = idleState,
        };

        fsm = new FSM(states);

        fsm.SetInitialState(typeof(IdleState));
    }

    private class IdleState : global::State
    {
        private BirdAnimator birdAnimator;

        public IdleState(BirdAnimator birdAnimator)
        {
            this.birdAnimator = birdAnimator;
        }

        public override void Enter()
        {
            birdAnimator.animator.SetInteger(birdAnimator.animatorStateHash, (int)State.Idle);
        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }
    }
}
