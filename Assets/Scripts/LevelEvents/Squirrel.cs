using UnityEngine;
using System.Collections;
using System;


public class Squirrel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform finalPosition;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject pivot;
    [SerializeField] private UnityPool stickPool;
    private EventBus eventBus;

    [Header("Config")]
    [SerializeField] private float reachFinalPosTime;
    [SerializeField] private float throwPower;
    [SerializeField] private float throwTorquePower;

    private const string peakAnimName = "Peak";
    private const string throwAnimName = "Throw";

    private Coroutine throwCoroutine = null;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        eventBus.Subscribe<OnSummonSquirrel>((Action)HandleOnSummon);
    }


    private IEnumerator ThrowCoroutine()
    {
        pivot.SetActive(true);

        float t = 0;

        Vector3 startingPos = transform.position;
        Vector3 finalPos = finalPosition.position;

        while (t < 1f)
        {
            t += Time.deltaTime / reachFinalPosTime;

            transform.position = Vector3.Lerp(startingPos, finalPos, t);

            yield return null;
        }

        transform.position = finalPos;

        animator.Play(peakAnimName);

        yield return null;

        yield return new WaitUntil(() => AnimationUtils.GetCurrentAnimationNormalizedTime(animator) >= 1f);

        animator.Play(throwAnimName);

        yield return new WaitUntil(() => AnimationUtils.GetCurrentAnimationNormalizedTime(animator) >= 1f);

        GameObject stick = stickPool.GetItem(throwPoint.position, Quaternion.identity, stickPool.transform);

        BasicProp stickPropComp = stick.GetComponent<BasicProp>();

        Vector3 playerDirection = Vector3.Normalize(player.transform.position - throwPoint.position);

        stickPropComp.Push(playerDirection * throwPower, Vector3.forward * throwTorquePower);

        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / reachFinalPosTime;

            transform.position = Vector3.Lerp(startingPos, finalPos, t);

            yield return null;
        }

        transform.position = startingPos;

        throwCoroutine = null;
    }

    private void HandleOnSummon()
    {
        if (throwCoroutine == null)
        {
            throwCoroutine = StartCoroutine(ThrowCoroutine());
        }
    }
}
