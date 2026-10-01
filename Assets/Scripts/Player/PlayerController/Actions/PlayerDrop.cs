using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDrop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private PlayerInventory playerInventory;
    private InputHandler inputHandle;
    private InputAction dropAction;
    private EventBus eventBus;

    private bool dropAnimFinished = false;

    private Coroutine dropCoroutine = null;

    private void Start()
    {
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        eventBus = serviceLocator.GetService<EventBus>();

        eventBus.Subscribe<OnPlayerDropAnimFinished>((Action)HandlePlayerDropAnimFinish);
        eventBus.Subscribe<OnPlayerTryDrop>((Action)HandlePlayerTryDrop);

        inputHandle = serviceLocator.GetService<InputHandler>();
        dropAction = inputHandle.playerInput.actions["Drop"];
        dropAction.performed += OnDrop;
    }

    private void OnDrop(InputAction.CallbackContext value)
    {
        if (enabled)
        {
            if (playerInventory.GetCurrentProp != null)
            {
                if (dropCoroutine == null)
                {
                    dropCoroutine = StartCoroutine(DropCoroutine());
                }
            }
        }
    }

    private IEnumerator DropCoroutine()
    {
        dropAnimFinished = false;

        eventBus.Raise<OnPlayerDrop>();

        while (!dropAnimFinished)
        {
            yield return null;

            Prop currentProp = playerInventory.GetCurrentProp;

            if (currentProp != null)
            {
                currentProp.transform.position = dropPoint.transform.position;
            }
        }

        dropCoroutine = null;
    }

    private void HandlePlayerTryDrop()
    {
        Prop currentProp = playerInventory.GetCurrentProp;

        if (currentProp != null)
        {
            if (dropCoroutine == null)
            {
                dropCoroutine = StartCoroutine(DropCoroutine());
            }
        }
    }

    private void HandlePlayerDropAnimFinish()
    {
        Prop currentProp = playerInventory.GetCurrentProp;

        if (currentProp != null)
        {
            currentProp.transform.position = dropPoint.position;
            currentProp.Enable();

            dropAnimFinished = true;
            currentProp = null;
        }
    }

    private void OnDestroy()
    {
        dropAction.performed -= OnDrop;
    }
}
