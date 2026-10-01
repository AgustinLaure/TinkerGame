using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>
{
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public void AddService<T>(T service) where T : class
    {
        var type = typeof(T);

        if (!_services.TryAdd(type, service))
        {
            Debug.LogWarning("Already subscribed: " + type);
        }
    }

    public void AddServiceAsGameObject<T>(Func<object[], bool> function, object[] param = null) where T : MonoBehaviour
    {
        GameObject gameObjectHandle = GameObject.Instantiate(new GameObject());
        gameObjectHandle.name = typeof(T).ToString();
        GameObject.DontDestroyOnLoad(gameObjectHandle);

        T service = gameObjectHandle.AddComponent<T>();

        AddService(service);

        if (function == null) return;

        if(param == null)
        {
            Debug.LogWarning($"{gameObjectHandle.name} had null params!");
            return;
        }

        if (function(param))
        {
            Debug.LogWarning($"{gameObjectHandle.name} couldn't run function");
        }
        return;
    }

    public void RemoveService<T>(T service) where T : class
    {
        var type = typeof(T);

        if (_services.ContainsKey(type))
        {
            _services.Remove(type);
        }
        else
        {
            Debug.LogWarning("Service doesnt exists: " + type);
        }
    }

    public T GetService<T>() where T : class
    {
        var type = typeof(T);

        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        Debug.LogWarning("Not suscribed: " + type);
        return null;
    }

    public void ClearServices() => _services.Clear();
}


