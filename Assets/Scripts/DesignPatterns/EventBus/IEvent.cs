using UnityEngine;

public interface IEvent : IReseteable
{
    public void Set(params object[] data);
}
