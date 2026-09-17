using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public event Action<Collider> OnTriggerEntered;
    public event Action<Collider> OnTriggerExited;

    [Header("References")]
    [SerializeField] private MonoScript triggerEnterEventRaise;
    [SerializeField] private MonoScript triggerExitEventRaise;

    [Header("Configuration")]
    [SerializeField] private bool passEnterCollisionData;
    [SerializeField] private bool passExitCollisionData;

    private EventBus eventBus;
    private MethodInfo genericTriggerEnterRaise;
    private MethodInfo genericTriggerExitRaise;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        if (triggerEnterEventRaise != null)
        {
            genericTriggerEnterRaise = eventBus.GetGenericRaiseMethod(triggerEnterEventRaise.GetClass());
        }
        if (triggerExitEventRaise != null)
        {
            genericTriggerExitRaise = eventBus.GetGenericRaiseMethod(triggerExitEventRaise.GetClass());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEntered?.Invoke(other);

        if (triggerEnterEventRaise != null)
        {
            if (passEnterCollisionData)
            {
                genericTriggerEnterRaise?.Invoke(eventBus, new object[] { other });
            }
            else
            {
                genericTriggerEnterRaise?.Invoke(eventBus, new object[] { new object[0] });
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExited?.Invoke(other);

        if (triggerExitEventRaise != null)
        {
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
}
