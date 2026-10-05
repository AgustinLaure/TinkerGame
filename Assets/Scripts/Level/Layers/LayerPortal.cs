using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LayerPortal : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private BoxCollider portalCollider;

    private EventBus eventBus;

    void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    void Start()
    {
        portalCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {

    }

    public void OnPortalEnter(GameObject obj)
    {
        eventBus.Raise<OnEnterLayerPortal>(target);

        obj.transform.position = target.position;
    }

    void OnTriggerEnter(Collider other)
    {
        OnPortalEnter(other.gameObject);
    }
}
