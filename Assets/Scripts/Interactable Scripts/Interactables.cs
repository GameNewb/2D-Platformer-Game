using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    [Header("Dummy Targets Properties")]
    [Space(2)]
    [SerializeField] public int health = 0;

    [Header("Currency Properties")]
    [Space(2)]
    [SerializeField] int points = 1;
    [SerializeField] bool addedToGold = false;
    
    private Animator animator;
    private Vector3 previousTransform;
    private bool isDamaged = false;
    private int previousHealth;
    private float animatorTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
        previousHealth = health;
        previousTransform = transform.position;

        // Obtain animator controller to determine the length of an animation
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == "Combat Dummy Hit") //If it has the same name as your clip
            {
                animatorTime = ac.animationClips[i].length;
            }
        }
    }

    private void Update()
    {
        // Object has been hit/damaged
        if (health != previousHealth)
        {
            animator.SetBool("IsDamaged", true);
            StartCoroutine("ChangeAnimation");
            previousHealth = health;
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player interacts with pickup object
        if (collision.gameObject.layer.Equals(10) && !gameObject.tag.Equals("Dummy Targets"))
        {
            if(!addedToGold)
            {
                addedToGold = true;
                FindObjectOfType<GameSession>().ProcessGold(points);
                Destroy(gameObject);
            }
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
