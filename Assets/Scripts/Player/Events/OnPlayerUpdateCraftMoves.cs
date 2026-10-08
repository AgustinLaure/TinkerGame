using System.Collections.Generic;

public class OnPlayerUpdateCraftMoves : IEvent
{
    public PlayerCraft.Move[] moves;

    public void Set(params object[] data)
    {
        moves = data[0] as PlayerCraft.Move[];
    }
    public void Reset()
    {
        moves = null;
    }
}
