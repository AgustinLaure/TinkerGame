using UnityEngine;

public class OnPickUpProp : IEvent
{
    public Prop prop;
    public void Set(params object[] data)
    {
        prop = data[0] as Prop;
    }
    public void Reset()
    {
        prop = null;
    }
}
