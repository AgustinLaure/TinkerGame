using UnityEngine;

public class OnRotatePlayer : IEvent
{
    public Quaternion rotation = Quaternion.identity;

    public void Set(params object[] data)
    {
        rotation = (Quaternion)data[0];
    }
    public void Reset()
    {
        rotation = Quaternion.identity;
    }
}
