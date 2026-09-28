using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoxCollider pickUpArea;
    private PlayerInput playerInput;
    private InputAction pickUpAction;
    private EventBus eventBus;

    [Header("Configs")]
    [SerializeField] private LayerMask propMask;

    private const int maxColliders = 20;

    private Collider[] propColliders = new Collider[maxColliders];

    private void Start()
    {
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        
        eventBus = serviceLocator.GetService<EventBus>();

        playerInput = serviceLocator.GetService<PlayerInput>();
        pickUpAction = playerInput.actions["PickUp"];
        pickUpAction.performed += OnPickUp;
    }

    private void OnPickUp(InputAction.CallbackContext value)
    {
        if (Physics.CheckBox(pickUpArea.bounds.center, pickUpArea.bounds.extents, pickUpArea.transform.rotation, propMask))
        {
            Physics.OverlapBoxNonAlloc(pickUpArea.bounds.center, pickUpArea.bounds.extents, propColliders, pickUpArea.transform.rotation, propMask);

            eventBus.Raise<OnPlayerPickUp>(propColliders[0].GetComponentInParent<Prop>());
        }
    }

    private void OnDestroy()
    {
        pickUpAction.performed -= OnPickUp;
    }
}
