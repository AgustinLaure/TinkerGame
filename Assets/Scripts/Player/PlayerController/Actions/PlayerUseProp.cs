using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseProp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;

    private void OnUseProp(InputValue value)
    {
        Prop currentProp = inventory.GetCurrentProp;

        if (currentProp != null)
        {
            currentProp.Action();
        }
    }
}
