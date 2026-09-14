using UnityEngine;

public class OnPlayerToggleCraft : IEvent
{
    public bool isCrafting = false;

    public void Set(params object[] data)
    {
        isCrafting = (bool)data[0];
    }
    public void Reset()
    {
        isCrafting = false;
    }
}
