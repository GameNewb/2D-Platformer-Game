using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructibles : MonoBehaviour
{
    [SerializeField] public GameObject itemToDrop;
    [SerializeField] public int health = 1;

    public void Update()
    {
        // Object has been hit/damaged
        if (health <= 0)
        {
            if (itemToDrop != null)
            {
                // Create a copy of the item and detach it from the destructible object
                Instantiate(itemToDrop, transform.position, Quaternion.identity, transform);
                transform.DetachChildren();
            }

            Destroy(gameObject);
        }
    }

}
