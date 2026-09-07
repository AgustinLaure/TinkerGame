using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    public List<MonoBehaviour> actions;

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
