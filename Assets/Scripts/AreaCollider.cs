using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class AreaCollider : MonoBehaviour
{
    public event Action<Collision> OnColliderEntered;
    public event Action<Collision> OnColliderExited;

    [Header("Event Type Names")]
    [SerializeField] private string colliderEnterEventTypeName;
    [SerializeField] private string colliderExitEventTypeName;

    [Header("Configuration")]
    [SerializeField] private bool passEnterCollisionData;
    [SerializeField] private bool passExitCollisionData;

    private EventBus eventBus;
    private MethodInfo genericTriggerEnterRaise;
    private MethodInfo genericTriggerExitRaise;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        if (!string.IsNullOrEmpty(colliderEnterEventTypeName))
        {
            genericTriggerEnterRaise = eventBus.GetGenericRaiseMethod(Type.GetType(colliderEnterEventTypeName));
        }

        if (!string.IsNullOrEmpty(colliderExitEventTypeName))
        {
            genericTriggerExitRaise = eventBus.GetGenericRaiseMethod(Type.GetType(colliderExitEventTypeName));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnColliderEntered?.Invoke(collision);

        if (passEnterCollisionData)
        {
            genericTriggerEnterRaise?.Invoke(eventBus, new object[] { collision });
        }
        else
        {
            genericTriggerEnterRaise?.Invoke(eventBus, new object[] { new object[0] });
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        OnColliderExited?.Invoke(collision);


        if (passExitCollisionData)
        {
            genericTriggerExitRaise?.Invoke(eventBus, new object[] { collision });
        }
        else
        {
            genericTriggerExitRaise?.Invoke(eventBus, new object[] { new object[0] });
        }
    }
}
