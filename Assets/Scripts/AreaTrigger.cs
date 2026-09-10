using System;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    private enum GlobalEvents
    {
        OnPlayerDetectedLand,
        OnPlayerDetectedFloor
    };

    public event Action<Collider> OnTriggerEntered;
    public event Action<Collider> OnTriggerExited;

    [Header("References")]
    [SerializeField] private GlobalEvents globalEvent;
    private EventBus eventBus;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEntered?.Invoke(other);

        switch (globalEvent)
        {
            case GlobalEvents.OnPlayerDetectedLand:
                eventBus.Raise<OnPlayerDetectedLand>();
                break;
            case GlobalEvents.OnPlayerDetectedFloor:
                eventBus?.Raise<OnPlayerDetectedFloor>();
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExited?.Invoke(other);
    }
}
