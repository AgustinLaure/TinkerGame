using UnityEngine;

public class OnEnterLayerPortal : IEvent
{
    private GameObject obj;
    private Transform target;

    public void Set(params object[] data)
    {
        obj = (GameObject)data[0];
        target = (Transform)data[1];
    }
    public void Reset()
    {
    }
}