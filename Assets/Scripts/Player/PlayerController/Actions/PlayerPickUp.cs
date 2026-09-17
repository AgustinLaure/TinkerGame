using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoxCollider pickUpArea;
    private EventBus eventBus;

    [Header("Configs")]
    [SerializeField] private LayerMask propMask;

    private const int maxColliders = 20;

    private Collider[] propColliders = new Collider[maxColliders];

    private void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    private void OnPickUp(InputValue value)
    {
        if (Physics.CheckBox(pickUpArea.bounds.center, pickUpArea.bounds.extents, pickUpArea.transform.rotation, propMask))
        {
            Physics.OverlapBoxNonAlloc(pickUpArea.bounds.center, pickUpArea.bounds.extents, propColliders, pickUpArea.transform.rotation, propMask);

            eventBus.Raise<OnPlayerPickUp>(propColliders[0].GetComponentInParent<Prop>());
        }
    }
}
