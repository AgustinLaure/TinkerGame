using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private EventBus eventBus;

    private Prop currentProp = null;

    public Prop GetCurrentProp { get { return currentProp; } }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        eventBus.Subscribe<OnPlayerPickUp>((Action<OnPlayerPickUp>)HandleOnPropPickUp);
        eventBus.Subscribe<OnPlayerPickUpAnimFinished>((Action)HandleOnPropPickedUp);
        eventBus.Subscribe<OnPlayerCraftedOrigami>((Action<OnPlayerCraftedOrigami>)HandlePlayerCraftOrigami);
        eventBus.Subscribe<OnAircraftLaunched>((Action)HandleAircraftLaunch);
        
    }

    private void HandleOnPropPickUp(OnPlayerPickUp data)
    {
        currentProp = data.prop;
    }

    private void HandleOnPropPickedUp()
    {
        currentProp.Disable();
    }

    private void HandlePlayerCraftOrigami(OnPlayerCraftedOrigami data)
    {
        currentProp = data.origami;
        currentProp.Disable();
    }

    private void HandleAircraftLaunch()
    {
        currentProp = null;
    }
}
