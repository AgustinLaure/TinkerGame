using UnityEngine;

public class OnPlayerMovedHorizontally : IEvent
{
    public float direction;

    public void Set(params object[] data)
    {
        data[0] = direction;
    }
    public void Reset()
    {
        direction = 0;
    }
}
