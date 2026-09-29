using System.Collections;
using UnityEngine;

public class Bush : PropSpawner
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Config")]
    [SerializeField] private float shakeCooldown;

    private AreaCollider areaCollider;

    private const string shakeAnimName = "Shake";

    private Coroutine shakeCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        areaCollider = GetComponent<AreaCollider>();

        areaCollider.OnColliderEntered += HandleAircraftCollision;
    }

    private void HandleAircraftCollision(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Aircraft>() != null)
        {
            if (shakeCoroutine == null)
            {
                shakeCoroutine = StartCoroutine(ShakeCoroutine());
            }
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        animator.Play(shakeAnimName);

        Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 361));

        Spawn(randomRotation);

        yield return new WaitForSeconds(shakeCooldown);

        shakeCoroutine = null;
    }

    private void SpawnObject()
    {

    }

    private void OnDestroy()
    {
        areaCollider.OnColliderEntered -= HandleAircraftCollision;
    }
}
