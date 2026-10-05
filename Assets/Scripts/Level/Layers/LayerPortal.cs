using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LayerPortal : MonoBehaviour
{
    [SerializeField]
    public Transform target;

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

    void OnTriggerEnter(Collider other)
    {
        Teletransportable tp = other.gameObject.GetComponent<Teletransportable>();
        if (!tp)
        {
            Debug.LogWarning("Non Teleportable object entered portal");
            return;
        }

        tp.EnterPortal(this);

        eventBus.Raise<OnEnterLayerPortal>(target);
    }
    void OnTriggerExit(Collider other)
    {
        Teletransportable tp = other.gameObject.GetComponent<Teletransportable>();
        if (!tp) return;

        tp.EnterPortal(this);

        eventBus.Raise<OnEnterLayerPortal>(target);
    }
}
