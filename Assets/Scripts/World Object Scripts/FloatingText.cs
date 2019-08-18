using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 3, 0);
    public float destroyTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
    }
    
}
