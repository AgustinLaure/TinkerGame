using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LayerPortal : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private BoxCollider collider;

    private EventBus eventBus;

    void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    void Update()
    {

    }
    void OnPortalEnter()
    {
        eventBus.Raise<OnEnterLayerPortal>(target);
    }
}
