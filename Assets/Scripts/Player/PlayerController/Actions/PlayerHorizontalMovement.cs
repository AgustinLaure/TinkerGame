using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHorizontalMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider legsCollider;
    [SerializeField] private PhysicsMaterial noFrictionMat;

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
        originalLegsColliderMat = legsCollider.sharedMaterial;
    }

    private void Update()
    {
        playerAxisInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(playerAxisInput.x * acceleration, 0f, 0f), walkForceMode);

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

    private void OnDisable()
    {
        SetLegsColliderSharedMaterial(originalLegsColliderMat);
    }
}
