using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public bool beingPushed;
    float xPosition;

    // Start is called before the first frame update
    void Start()
    {
        xPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Make the object move only when necessary
        if (beingPushed == false)
        {
            transform.position = new Vector3(xPosition, transform.position.y);
        }
        else
        {
            xPosition = transform.position.x;
        }
    }
}
