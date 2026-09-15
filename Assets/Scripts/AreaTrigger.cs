using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public event Action<Collider> OnTriggerEntered;
    public event Action<Collider> OnTriggerExited;

    [Header("Event Type Names")]
    [SerializeField] private string triggerEnterEventTypeName;
    [SerializeField] private string triggerExitEventTypeName;

    [Header("Configuration")]
    [SerializeField] private bool passEnterCollisionData;
    [SerializeField] private bool passExitCollisionData;

    private EventBus eventBus;
    private MethodInfo genericTriggerEnterRaise;
    private MethodInfo genericTriggerExitRaise;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        if (!string.IsNullOrEmpty(triggerEnterEventTypeName))
        {
            genericTriggerEnterRaise = eventBus.GetGenericRaiseMethod(Type.GetType(triggerEnterEventTypeName));
        }

        if (!string.IsNullOrEmpty(triggerExitEventTypeName))
        {
            genericTriggerExitRaise = eventBus.GetGenericRaiseMethod(Type.GetType(triggerExitEventTypeName));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEntered?.Invoke(other);

        if (passEnterCollisionData)
        {
            genericTriggerEnterRaise?.Invoke(eventBus, new object[] { other });
        }
        else
        {
            genericTriggerEnterRaise?.Invoke(eventBus, new object[] { new object[0] });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExited?.Invoke(other);

        if (passExitCollisionData)
        {
            genericTriggerExitRaise?.Invoke(eventBus, new object[] { other });
        }
        else
        {
            genericTriggerExitRaise?.Invoke(eventBus, new object[] { new object[0] });
        }
    }
}
