using System.Collections.Generic;


public class Pool
{
    public Pool()
    {
        stack = new Stack<IReseteable>();
    }

    private Stack<IReseteable> stack;

    public T GetItem<T>() where T : IReseteable, new()
    {
        if (stack.Count > 0)
        {
            return (T)stack.Pop();
        }
        else
        {
            return new T();
        }
    }

    public void ReturnItem<T>(T item) where T : IReseteable
    {
        item.Reset();
        stack.Push(item);
    }
}

