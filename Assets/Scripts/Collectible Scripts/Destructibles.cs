using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructibles : MonoBehaviour
{
    [SerializeField] public GameObject itemToDrop;
    [SerializeField] public int health = 1;

    private Animator animator;
    private bool itemDestroyed = false;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        // Object has been hit/damaged
        if (health <= 0)
        {
            StartCoroutine("DropItem");
        }
    }

    IEnumerator DropItem()
    {
        if (itemToDrop != null && !itemDestroyed)
        { 
            if (animator != null)
            {
                animator.SetBool("IsDestroyed", true);
            }

            // Create a copy of the item and detach it from the destructible object
            itemDestroyed = true;
            Instantiate(itemToDrop, transform.position, Quaternion.identity, transform);
            transform.DetachChildren();
        }

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

}
