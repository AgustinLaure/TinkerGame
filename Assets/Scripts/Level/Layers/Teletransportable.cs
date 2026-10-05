using UnityEngine;

public class Teletransportable : MonoBehaviour
{
    private bool telelportReady = true;

    public void EnterPortal(LayerPortal portal)
    {
        if (!telelportReady) return;
        telelportReady = false;

        transform.position = portal.target.position;
    }

    public void ExitPortal(LayerPortal portal)
    {
        if (telelportReady) return;
        telelportReady = true;
    }
}
