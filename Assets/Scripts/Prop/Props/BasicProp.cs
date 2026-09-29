using UnityEngine;

public class BasicProp : Prop
{
    [Header("References")]
    [SerializeField] private GameObject rendererObject;
    [SerializeField] private GameObject colliderObject;

    private Rigidbody rb;

    private const ForceMode pushForceMode = ForceMode.Impulse;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
            rb.AddForce(push,pushForceMode);
        }

        if (torque != Vector3.zero)
        {
            rb.AddTorque(torque,pushForceMode);
        }
    }

    public override void Disable()
    {
        DisableRigidBody(rb);
        rendererObject.SetActive(false);
        colliderObject.SetActive(false);
    }
}
