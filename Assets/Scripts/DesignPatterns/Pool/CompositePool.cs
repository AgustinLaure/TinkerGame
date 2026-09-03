using System;
using System.Collections.Generic;
using UnityEngine;

public class CompositePool
{
    private class AddExistingPool : Exception
    {
        public AddExistingPool(string message) : base(message)
        {

        }
    }

    private class RemoveNonExistingPool : Exception
    {
        public RemoveNonExistingPool(string message) : base(message)
        {

        }
    }

    private Dictionary<Type, Pool> compositePool;

    public CompositePool()
    {
        compositePool = new Dictionary<Type, Pool>();
    }

    public void AddPool<T>()
    {

        try
        {
            if (compositePool.ContainsKey(typeof(T)))
            {
                throw new AddExistingPool("Tried to add an already existing pool");
            }
            else
            {
                compositePool.TryAdd(typeof(T), new Pool());
            }
        }
        catch (AddExistingPool exception)
        {
            Debug.LogWarning(exception.Message);
        }
    }

    public void RemovePool<T>()
    {
        try
        {
            if (compositePool.ContainsKey(typeof(T)))
            {
                compositePool.Remove(typeof(T));
            }
            else
            {
                throw new RemoveNonExistingPool("Tried to remove a non existing pool");
            }
        }
        catch (RemoveNonExistingPool exception)
        {
            Debug.LogWarning(exception.Message);
        }
    }

    public T GetItemFromPool<T>() where T : IReseteable, new()
    {
        if (compositePool.TryGetValue(typeof(T), out Pool pool))
        {
            return pool.GetItem<T>();
        }
        else
        {
            Pool newPool = new Pool();
            compositePool.TryAdd(typeof(T), newPool);

            return newPool.GetItem<T>();
        }
    }

    public void ReturnItemFromPool<T>(T item) where T : IReseteable, new()
    {
        if (compositePool.TryGetValue(typeof(T), out Pool pool))
        {
            pool.ReturnItem(item);
        }
        else
        {
            Pool newPool = new Pool();
            compositePool.TryAdd(typeof(T), newPool);

            newPool.ReturnItem(item);
        }
    }

    public void Clear()
    {
        compositePool.Clear();
    }
}
