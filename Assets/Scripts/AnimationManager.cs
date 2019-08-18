using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator animator;

    [HideInInspector] public float animatorTime = 0f;

    public float GetAnimationLength(string animationName)
    {
        // Obtain animator controller to determine the length of an animation
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            // If it has the same name as the clip provided, retrieve its length
            if (ac.animationClips[i].name == animationName) 
            {
                animatorTime = ac.animationClips[i].length;
            }
        }

        return animatorTime;
    }
}
