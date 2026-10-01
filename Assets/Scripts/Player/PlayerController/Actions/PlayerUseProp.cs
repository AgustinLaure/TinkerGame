using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseProp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    private InputHandler inputHandle;
    private InputAction usePropAction;

    private void Start()
    {
        inputHandle = ServiceLocator.Instance.GetService<InputHandler>();
        usePropAction = inputHandle.playerInput.actions["UseProp"];

        usePropAction.performed += OnUseProp;
    }

    private void OnUseProp(InputAction.CallbackContext value)
    {
        if (enabled)
        {
            Prop currentProp = inventory.GetCurrentProp;

            if (currentProp != null)
            {
                currentProp.Action();
            }
        }
    }

    private void OnDestroy()
    {
        usePropAction.performed -= OnUseProp;
    }
}
