using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 0, 0);
    public Vector3 randomizedIntensity = new Vector3(0.1f, 0.6f, 0);
    public float destroyTime = 1f;
    
    void Start()
    {
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-randomizedIntensity.x, randomizedIntensity.x), 
            Random.Range(-randomizedIntensity.y, randomizedIntensity.y), 
            Random.Range(-randomizedIntensity.z, randomizedIntensity.z));
    }

}
