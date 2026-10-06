using System.Collections;
using UnityEngine;

public class Nest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] sticksTransform;
    [SerializeField] private AreaTrigger collectionArea;
    private EventBus eventBus;

    [Header("Config")]
    [SerializeField] private float snapTime;

    private BasicProp[] sticks;
    private int stickCount = 0;

    private const string stickTag = "Stick";

    private void Awake()
    {
        sticks = new BasicProp[sticksTransform.Length];
        collectionArea.OnTriggerEntered += HandleCollectionAreaTrigger;
    }

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
    }

    private void OnDestroy()
    {
        collectionArea.OnTriggerEntered -= HandleCollectionAreaTrigger;
    }

    private IEnumerator AddStickCoroutine(BasicProp prop, int transformIndex)
    {
        prop.DisablePhysics();

        yield return SnapCoroutine(prop, transformIndex);
    }

    private IEnumerator SnapCoroutine(BasicProp prop, int transformIndex)
    {
        float t = 0;

        Vector3 initialPos = prop.transform.position;
        Quaternion initialRotation = prop.transform.rotation;
        Transform propTRS = prop.transform;

        while (t < 1f)
        {
            t += Time.deltaTime / snapTime;

            propTRS.position = Vector3.Lerp(initialPos, sticksTransform[transformIndex].position, t);
            propTRS.rotation = Quaternion.Slerp(initialRotation, sticksTransform[transformIndex].rotation, t);

            yield return null;
        }

        propTRS.position = sticksTransform[transformIndex].position;
    }

    private void HandleCollectionAreaTrigger(Collider collider)
    {
        GameObject collisionGameObject = collider.gameObject;

        if (collisionGameObject.CompareTag(stickTag))
        {
            if (stickCount < sticksTransform.Length)
            {
                StartCoroutine(AddStickCoroutine(collisionGameObject.GetComponentInParent<BasicProp>(), stickCount));
                stickCount++;

                if (stickCount == sticksTransform.Length)
                {
                    eventBus.Raise<OnNestCompleted>();
                }
            }
        }
    }
}
