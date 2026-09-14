using UnityEngine;

public abstract class Prop : MonoBehaviour, IPickable
{

    public virtual void Action()
    {

    }

    public virtual void Discard()
    {

    }

    public abstract void EnableLogic();

    public abstract void DisableLogic();

    public void SetHighlight(bool state)
    {

    }

    protected void EnableRigidBody(Rigidbody rb)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }

    protected void DisableRigidbody(Rigidbody rb)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.detectCollisions = false;
    }
}
