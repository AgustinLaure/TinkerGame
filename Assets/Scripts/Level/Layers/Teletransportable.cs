using UnityEngine;

public class Teletransportable : MonoBehaviour
{
    [SerializeField] private Transform tpTransform = null;

    [SerializeField] private Rigidbody rb;


    private bool telelportReady = true;

    private void Start()
    {
        if (!tpTransform) tpTransform = transform;

        if (!rb) rb = tpTransform.GetComponent<Rigidbody>();

        if (!rb) Debug.LogWarning("No attached rigidbody to teleportable!");
    }

    public void EnterPortal(LayerPortal portal)
    {
        if (!telelportReady) return;
        telelportReady = false;

        tpTransform.position = portal.target.transform.position;

        if (portal.flipSpeedOnTeleport)
        {
            if (!rb)
            {
                Debug.LogError("tried flpping teleportable but no rigidbody was found!");
                return;
            }
            rb.linearVelocity *= new Vector2(-1.0f,1.0f);
        }
    }

    public void ExitPortal(LayerPortal portal)
    {
        if (telelportReady) return;
        telelportReady = true;
    }
}
