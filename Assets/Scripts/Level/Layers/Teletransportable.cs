using UnityEngine;

public class Teletransportable : MonoBehaviour
{
    [SerializeField] private Transform tpTransform = null;

    [SerializeField] private Rigidbody rb;

    private int layerIndex = -1;
    public int LayerIndex { get { return layerIndex; } }
    public void SetLayerIndex(int newIndex) { layerIndex = newIndex; }

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

        Vector3 newPos = portal.target.transform.position;

        newPos.y += tpTransform.position.y - portal.transform.position.y;

        tpTransform.position = newPos;

        if (portal.flipSpeedOnTeleport)
        {
            if (!rb)
            {
                Debug.LogError("tried flpping teleportable but no rigidbody was found!");
                return;
            }
            rb.linearVelocity *= new Vector2(-1.0f,1.0f);
        }

        if (portal.multiplySpeedOnTeleport != 1.0f)
        {
            if (!rb)
            {
                Debug.LogError("tried speeding up teleportable but no rigidbody was found!");
                return;
            }
            rb.linearVelocity *= portal.multiplySpeedOnTeleport;
        }

        SetLayerIndex(portal.target.LayerIndex);

        ServiceLocator.Instance.GetService<GameManager>().levelManager.SetCurrentLayer(portal.target.LayerIndex);
    }

    public void ExitPortal(LayerPortal portal)
    {
        if (telelportReady) return;
        telelportReady = true;
    }
}
