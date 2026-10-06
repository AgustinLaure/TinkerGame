using UnityEngine;

public class WaterFlower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AreaCollider areaCollider;
    private EventBus eventBus;

    private const string aircraftTag = "Aircraft";

    private void Awake()
    {
        areaCollider.OnColliderEntered += HandleAreaCollision;
    }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    private void HandleAreaCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag(aircraftTag))
        {
            eventBus.Raise<OnWaterFlowerKnocked>();
        }
    }

    private void OnDestroy()
    {
        areaCollider.OnColliderEntered -= HandleAreaCollision;
    }
}
