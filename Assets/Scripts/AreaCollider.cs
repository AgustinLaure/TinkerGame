using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class AreaCollider : MonoBehaviour
{
    public event Action<Collision> OnColliderEntered;
    public event Action<Collision> OnColliderExited;

    [Header("References")]
    [SerializeField] private MonoScript colliderEnterEventRaise;
    [SerializeField] private MonoScript colliderExitEventRaise;

    [Header("Configuration")]
    [SerializeField] private bool passEnterCollisionData;
    [SerializeField] private bool passExitCollisionData;

    private EventBus eventBus;
    private MethodInfo genericTriggerEnterRaise;
    private MethodInfo genericTriggerExitRaise;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        if (colliderEnterEventRaise != null)
        {
            genericTriggerEnterRaise = eventBus.GetGenericRaiseMethod(colliderEnterEventRaise.GetClass());
        }
        if (colliderExitEventRaise != null)
        {
            genericTriggerExitRaise = eventBus.GetGenericRaiseMethod(colliderExitEventRaise.GetClass());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnColliderEntered?.Invoke(collision);

        if (colliderEnterEventRaise != null)
        {
            if (passEnterCollisionData)
            {
                genericTriggerEnterRaise?.Invoke(eventBus, new object[] { collision });
            }
            else
            {
                genericTriggerEnterRaise?.Invoke(eventBus, new object[] { new object[0] });
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        OnColliderExited?.Invoke(collision);

        if (colliderExitEventRaise != null)
        {
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
}
