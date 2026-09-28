using UnityEngine;

public static class AnimationUtils
{
    public static bool GetCurrentAnimationEnded(Animator animator, int currentAnimHash)
    {
        AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);

        return animatorInfo.normalizedTime >= 1f && animatorInfo.shortNameHash == currentAnimHash;
    }

    public static float GetCurrentAnimationNormalizedTime(Animator animator)
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public static float GetCurrentAnimationLength(Animator animator)
    {
        return animator.GetCurrentAnimatorClipInfo(0).Length;
    }

    //public static void AccelerateCurrentAnimation(Animator animator, float duration)
    //{
    //    float animLength = GetCurrentAnimationLength();
    //    float currentNormTime = GetCurrentAnimatioNNormalizedTime();
    //
    //    float remainingTime = animLength - animLength * currentNormTime;
    //
    //    if (remainingTime > duration)
    //    {
    //        animator.SetFloat(animSpeedHash, remainingTime * (1f / duration));
    //    }
    //}
}
