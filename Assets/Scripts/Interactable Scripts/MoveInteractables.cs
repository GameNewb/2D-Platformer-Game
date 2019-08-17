using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInteractables : MonoBehaviour
{
    Vector3 newPlayerVector;
    Vector3 newLocalScale;
    public LayerMask objectMask;
    public float moveDistance = 1f;

    GameObject interactableObject;

    // Update is called once per frame
    void Update()
    {
        // Adjustable player transform 
        newPlayerVector = new Vector3(transform.position.x, transform.position.y / 1.1f, transform.position.z);
        newLocalScale = transform.localScale;

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(newPlayerVector, Vector2.right * newLocalScale.x, moveDistance, objectMask);

        // If player is hitting the box and key is being pressed 
        if (hit.collider != null && Input.GetKeyDown(KeyCode.E))
        {
            interactableObject = hit.collider.gameObject;

            interactableObject.GetComponent<FixedJoint2D>().enabled = true;
            interactableObject.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();

            // Only set this value once to prevent numerous synchronous calls
            if (interactableObject.GetComponent<MovingObject>().beingPushed == false)
            {
                interactableObject.GetComponent<MovingObject>().beingPushed = true;
            }

        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            if (interactableObject)
            {
                interactableObject.GetComponent<FixedJoint2D>().enabled = false;
                interactableObject.GetComponent<MovingObject>().beingPushed = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(newPlayerVector, (Vector2) newPlayerVector + Vector2.right * newLocalScale.x * moveDistance);
    }
}
