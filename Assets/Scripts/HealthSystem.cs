using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public Image[] heartImage;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        for (int i = 0; i < heartImage.Length; i++)
        {
            // Display the current health
            if (i < currentHealth)
            {
                heartImage[i].sprite = fullHeart;
            }
            else
            {
                heartImage[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                // Make heart visible
                // Only run code when it is set
                // Prevent MissingReferenceException when player object is destroyed / killed
                if (heartImage[i] != null)
                {
                    heartImage[i].enabled = true;
                }
            }
            else
            {
                // Hide heart image
                if (heartImage[i] != null)
                {
                    heartImage[i].enabled = false;
                }
            }
        }
        
    }
}
