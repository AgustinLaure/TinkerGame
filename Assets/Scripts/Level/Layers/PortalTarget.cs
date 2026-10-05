using System.Collections.Generic;

using UnityEngine;

public class PortalTarget : MonoBehaviour
{
    private Transform target;

    private List<LayerPortal> portals = new List<LayerPortal>();


    private int layerIndex = -1;

    public int LayerIndex { get { return layerIndex; } }
    public void SetLayerIndex(int newIndex) { layerIndex = newIndex; }

    private void Awake()
    {
        target = transform;
    }

    public void AddPortal(LayerPortal portal)
    {
        portals.Add(portal);
    }
}
