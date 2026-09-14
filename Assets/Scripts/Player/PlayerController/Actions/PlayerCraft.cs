using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCraft : MonoBehaviour
{
    private EventBus eventBus;

    private bool isCrafting = false;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    private void OnToggleCraft(InputValue value)
    {
        isCrafting = !isCrafting;

        eventBus.Raise<OnPlayerToggleCraft>(isCrafting);
    }
}
