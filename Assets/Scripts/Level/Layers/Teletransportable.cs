using UnityEngine;

public class Teletransportable : MonoBehaviour
{
    [SerializeField] private Transform tpTransform = null;

    private bool telelportReady = true;

    private void Start()
    {
        if (!tpTransform) tpTransform = transform;
    }

    public void EnterPortal(LayerPortal portal)
    {
        if (!telelportReady) return;
        telelportReady = false;

        tpTransform.position = portal.target.transform.position;
    }

    public void ExitPortal(LayerPortal portal)
    {
        if (telelportReady) return;
        telelportReady = true;
    }
}
