using UnityEngine;

public class BasicProp : Prop
{
    [Header("References")]
    [SerializeField] private GameObject rendererObject;
    [SerializeField] private GameObject colliderObject;
    private AreaCollider areaCollider;
    private Rigidbody rb;

    private const ForceMode pushForceMode = ForceMode.Impulse;

    public AreaCollider GetAreaCollider { get { return areaCollider; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (gameObject.TryGetComponent<AreaCollider>(out AreaCollider collider))
        {
            areaCollider = collider;
        }
        else
        {
            areaCollider = gameObject.AddComponent<AreaCollider>();
        }
    }

    public override void Enable()
    {
        EnableRigidBody(rb);
        rendererObject.SetActive(true);
        colliderObject.SetActive(true);
    }

    public void Push(Vector3 push, Vector3 torque)
    {
        if (push != Vector3.zero)
        {
            rb.AddForce(push, pushForceMode);
        }

        if (torque != Vector3.zero)
        {
            rb.AddTorque(torque, pushForceMode);
        }
    }

    public override void Disable()
    {
        DisableRigidBody(rb);
        rendererObject.SetActive(false);
        colliderObject.SetActive(false);
    }

    public void SetGravity(bool state)
    {
        rb.useGravity = state;
    }

    public void SetKinematic(bool state)
    {
        rb.isKinematic = state;
    }

    public void EnablePhysics()
    {
        EnableRigidBody(rb);
    }

    public void DisablePhysics()
    {
        DisableRigidBody(rb);
    }
}
