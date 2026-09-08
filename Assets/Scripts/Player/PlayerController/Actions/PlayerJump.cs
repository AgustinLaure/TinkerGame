using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float jumpImpulse;
    [SerializeField] private float delay;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody rb;
    private EventBus eventBus;

    private ForceMode forceMode = ForceMode.Impulse;

    private ForceMode forceMode = ForceMode.Impulse;

    private bool isJumpRequested = false;

    private Vector3 jumpDirection = Vector3.up;

    private Coroutine jumpCoroutine = null;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    private void FixedUpdate()
    {
        if (isJumpRequested)
        {
            rb.AddForce(jumpDirection * jumpImpulse, forceMode);
            isJumpRequested = false;
        }
    }

    private IEnumerator JumpCoroutine()
    {
        eventBus.Raise<OnPlayerJump>();

        yield return new WaitForSeconds(delay);

        isJumpRequested = true;

        jumpCoroutine = null;
    }

    private void OnJump(InputValue value)
    {
        if (jumpCoroutine == null)
        {
            jumpCoroutine = StartCoroutine(JumpCoroutine());
        }
    }
}