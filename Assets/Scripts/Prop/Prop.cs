using UnityEngine;

public abstract class Prop : IPickable
{

    public virtual void Action()
    {

    }

    public virtual void Discard()
    {

    }

    public abstract void SetLogic(bool state);

    public void SetHighlight(bool state)
    {

    }
}
