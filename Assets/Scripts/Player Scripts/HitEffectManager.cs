using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitEffectManager : MonoBehaviour
{
    public void ShowHitEffects(GameObject hitEffectPrefab)
    {
        if (hitEffectPrefab)
        {
            // Show the hit effect prefab
            var hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity, transform);
            hitEffect.SetActive(true);
        }
    }

}
