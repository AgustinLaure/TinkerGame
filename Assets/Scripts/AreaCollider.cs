using System;
using UnityEngine;

public class AreaCollider : MonoBehaviour
{
    public event Action<Collision> OnColliderEntered;
    public event Action<Collision> OnColliderExited;

    private void OnCollisionEnter(Collision collision)
    {
        OnColliderEntered?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnColliderExited?.Invoke(collision);
    }
}
