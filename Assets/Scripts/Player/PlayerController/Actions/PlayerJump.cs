using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float jumpImpulse;
    [SerializeField] private float delay;
    [SerializeField] private float jumpCooldown;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    private PlayerInput playerInput;
    private EventBus eventBus;
    private InputAction jumpAction;

    private ForceMode forceMode = ForceMode.Impulse;

    private bool isJumpRequested = false;

    private Vector3 jumpDirection = Vector3.up;

    private Coroutine jumpCoroutine = null;


    private void Start()
    {
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        eventBus = serviceLocator.GetService<EventBus>();
        playerInput = serviceLocator.GetService<PlayerInput>();

        jumpAction = playerInput.actions["Jump"];

        jumpAction.performed += OnJump;
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

        yield return new WaitForSeconds(jumpCooldown);

        jumpCoroutine = null;
    }

    private void OnJump(InputAction.CallbackContext value)
    {
        if (jumpCoroutine == null && enabled == true)
        {
            jumpCoroutine = StartCoroutine(JumpCoroutine());
        }
    }

    private void OnDestroy()
    {
        jumpAction.performed -= OnJump;
    }
}