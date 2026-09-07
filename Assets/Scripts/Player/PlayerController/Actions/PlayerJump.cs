using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    private ForceMode forceMode = ForceMode.Impulse;

    [Header("Values")]
    [SerializeField] private float jumpImpulse;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody rb;

    private bool isJumpRequested = false;

    private Vector3 jumpDirection = Vector3.up;

    private void FixedUpdate()
    {
        if (isJumpRequested)
        {
            rb.AddForce(jumpDirection * jumpImpulse, forceMode);
            isJumpRequested = false;
        }
    }

    private void OnJump(InputValue value)
    {
        isJumpRequested = true;
    }
}
