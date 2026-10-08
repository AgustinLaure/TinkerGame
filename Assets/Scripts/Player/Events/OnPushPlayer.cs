using UnityEngine;

public class OnPushPlayer : IEvent
{
    public Vector3 push;

    public void Set(params object[] data)
    {
        push = (Vector3)data[0];
    }

    public void Reset()
    {
        push = Vector3.zero;
    }
}
