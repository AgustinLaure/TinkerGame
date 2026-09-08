using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHorizontalMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider legsCollider;
    [SerializeField] private PhysicsMaterial noFrictionMat;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private EventBus eventBus;

    [Header("Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float terminalVelocity;

    private const ForceMode walkForceMode = ForceMode.Acceleration;

    private Vector2 playerAxisInput = Vector2.zero;
    private InputAction moveAction;

    private PhysicsMaterial originalLegsColliderMat;

    private void Awake()
    {
        moveAction = playerInput.actions["Move"];
    }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        originalLegsColliderMat = legsCollider.sharedMaterial;
    }

    private void Update()
    {
        playerAxisInput = moveAction.ReadValue<Vector2>();

        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        if (playerAxisInput.x != 0f)
        {
            rb.AddForce(new Vector3(playerAxisInput.x * acceleration, 0f, 0f), walkForceMode);
            eventBus.Raise<OnPlayerMovedHorizontally>(playerAxisInput.x);
        }

        rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -terminalVelocity, terminalVelocity), rb.linearVelocity.y, rb.linearVelocity.z);

        if (playerAxisInput.magnitude <= 0f)
        {
            SetLegsColliderSharedMaterial(originalLegsColliderMat);
        }
        else
        {
            SetLegsColliderSharedMaterial(noFrictionMat);
        }
    }

    private void SetLegsColliderSharedMaterial(PhysicsMaterial physicsMaterial)
    {
        if (legsCollider.sharedMaterial != physicsMaterial)
        {
            legsCollider.sharedMaterial = physicsMaterial;
        }
    }

    private void UpdateFacingDirection()
    {
        float horizontalInput = playerAxisInput.x;

        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnDisable()
    {
        SetLegsColliderSharedMaterial(originalLegsColliderMat);
    }
}
