using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDrop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform dropPoint;
    private PlayerInput playerInput;
    private InputAction dropAction;
    private EventBus eventBus;

    private Prop currentProp = null;


    private void Start()
    {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        eventBus = serviceLocator.GetService<EventBus>();

        eventBus.Subscribe<OnPlayerDropAnimFinished>((Action)HandlePlayerDropAnimFinish);
        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)HandlePlayerPickUp);

        playerInput = serviceLocator.GetService<PlayerInput>();
        dropAction = playerInput.actions["Drop"];
        dropAction.performed += OnDrop;
    }

    private void OnDrop(InputAction.CallbackContext value)
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

    private void OnDestroy()
    {
        dropAction.performed -= OnDrop;
    }
}
