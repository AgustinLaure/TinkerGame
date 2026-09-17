
public class OnPlayerCraftedOrigami : IEvent
{
    public Origami origami;

    public void Set(params object[] data)
    {
        origami = data[0] as Origami;
    }
    public void Reset()
    {
        origami = null;
    }
}
