using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private EventBus eventBus;

    private Prop currentItem = null;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        eventBus.Subscribe<OnPickUpProp>((Action<OnPickUpProp>)HandleOnPropPickUp);
        eventBus.Subscribe<OnPickedUpProp>((Action)HandleOnPropPickedUp);
    }

    private void HandleOnPropPickUp(OnPickUpProp data)
    {
        currentItem = data.prop;
    }

    private void HandleOnPropPickedUp()
    {

    }
}
