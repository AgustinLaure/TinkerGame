using UnityEngine;

public class OnEnterLayerPortal : IEvent
{
    private Transform target;

    public void Set(params object[] data)
    {
        target = (Transform)data[0];
    }
    public void Reset()
    {
    }
}