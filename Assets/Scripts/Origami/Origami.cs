using System.Collections;
using UnityEngine;

public abstract class Origami : Prop
{
    [Header("OrigamiRefs")]
    [SerializeField] private SphereCollider sphereCollilder;
    [SerializeField] private GameObject baseForm;
    [SerializeField] private GameObject crumpledForm;
    [SerializeField] private Animation legacyAnim;

    protected string crumpleClipName;
    protected bool isCrumpled = false;

    private Coroutine crumpleCoroutine = null;
    protected float crumpleColliderSize = 1f; 

    protected virtual void Crumple()
    {
        if (crumpleCoroutine == null)
        {
            crumpleCoroutine = StartCoroutine(CrumpleCoroutine());
        }
    }

    private IEnumerator CrumpleCoroutine()
    {
        isCrumpled = true;

        legacyAnim.Play(crumpleClipName);

        yield return new WaitUntil(() => !legacyAnim.isPlaying);

        sphereCollilder.GetComponent<SphereCollider>().radius = crumpleColliderSize;
    }
}
