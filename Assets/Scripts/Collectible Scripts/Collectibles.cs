using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [Header("Currency Properties")]
    [Space(2)]
    [SerializeField] int points = 1;
    [SerializeField] bool addedToGold = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player interacts with pickup object
        if (collision.gameObject.layer.Equals(10))
        {
            if (!addedToGold)
            {
                addedToGold = true;
                FindObjectOfType<GameSession>().ProcessGold(points);
                Destroy(gameObject);
            }
        }
    }

}
