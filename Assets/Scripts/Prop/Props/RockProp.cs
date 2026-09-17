using UnityEngine;

public class RockProp : Prop
{
    [Header("References")]
    [SerializeField] private GameObject rendererObject;
    [SerializeField] private GameObject colliderObject;

    private Rigidbody rb;

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

    public override void Disable()
    {
        DisableRigidbody(rb);
        rendererObject.SetActive(false);
        colliderObject.SetActive(false);
    }
}
