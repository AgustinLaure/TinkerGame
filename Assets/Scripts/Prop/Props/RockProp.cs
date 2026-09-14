using UnityEngine;

public class RockProp : Prop
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void EnableLogic()
    {
        EnableRigidBody(rb);
    }

    public override void DisableLogic()
    {
        DisableRigidbody(rb);
    }
}
