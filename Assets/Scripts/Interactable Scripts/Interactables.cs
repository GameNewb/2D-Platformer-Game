using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    [Header("Interactable Properties")]
    [Space(2)]
    [SerializeField] public int health = 0;

    private FloatingTextManager textManager;
    private Animator animator;
    private Vector3 previousTransform;
    private int previousHealth;
    private float animatorTime;

    private void Start()
    {
        textManager = GetComponent<FloatingTextManager>();
        animator = GetComponent<Animator>();
        animatorTime = GetComponent<AnimationManager>().GetAnimationLength("Combat Dummy Hit");
        previousHealth = health;
        previousTransform = transform.position;
    }

    private void Update()
    {
        // Object has been hit/damaged
        if (health != previousHealth)
        {
            animator.SetBool("IsDamaged", true);
            StartCoroutine("ChangeAnimation");
            previousHealth = health;
            //textManager.ShowFloatingText(previousHealth.ToString());
        }
    }

    IEnumerator ChangeAnimation()
    {
        // Move position by a tiny bit when hit
        transform.position = new Vector3(transform.position.x + 0.12f, transform.position.y + 0.03f, 0);
        yield return new WaitForSeconds(animatorTime);

        // Reset back to idle
        animator.SetBool("IsDamaged", false);
        transform.position = previousTransform;
    }
}
