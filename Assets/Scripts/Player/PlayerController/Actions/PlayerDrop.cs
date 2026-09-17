using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDrop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform dropPoint;
    private EventBus eventBus;

    private Prop currentProp = null;

    private void Awake()
    {

    }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        eventBus.Subscribe<OnPlayerDropAnimFinished>((Action)HandlePlayerDropAnimFinish);
        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)HandlePlayerPickUp);
    }

    private void OnDrop(InputValue value)
    {
        if (currentProp != null)
        {
            eventBus.Raise<OnPlayerDrop>();
        }
    }

    private void HandlePlayerPickUp(OnPlayerPickUp data)
    {
        currentProp = data.prop;
    }

    private void HandlePlayerDropAnimFinish()
    {
        currentProp.transform.position = dropPoint.position;
        currentProp.Enable();

        currentProp = null;
    }
}
