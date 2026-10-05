using System.Collections.Generic;

using UnityEngine;

public class PortalTarget : MonoBehaviour
{
    private Transform target;

    private List<LayerPortal> portals = new List<LayerPortal>();

    private void Awake()
    {
        target = transform;
    }

    public void AddPortal(LayerPortal portal)
    {
        portals.Add(portal);
    }
}
