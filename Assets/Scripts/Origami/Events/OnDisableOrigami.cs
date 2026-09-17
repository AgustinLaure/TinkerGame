using UnityEngine;
using System;

public class OnDisableOrigami : IEvent
{
    public GameObject gameObject = null;
    public Type type = null;

    public void Set(params object[] data)
    {
        gameObject = data[0] as GameObject;
        type = data[1] as Type;
    }
    public void Reset()
    {
        gameObject = null;
        type = null;
    }
}
