using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private EventBus eventBus;

    private Prop currentItem = null;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)HandleOnPropPickUp);
        eventBus.Subscribe<OnPlayerPickUpAnimFinished>((Action)HandleOnPropPickedUp);
    }

    private void HandleOnPropPickUp(OnPlayerPickUp data)
    {
        currentItem = data.prop;
    }

    private void HandleOnPropPickedUp()
    {
        currentItem.Disable();
    }
}
