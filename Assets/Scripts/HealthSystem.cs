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

        if (heartImage != null)
        {
            for (int i = 0; i < heartImage.Length; i++)
            {
                // Only run code when it is set
                // Prevent MissingReferenceException when player object is destroyed / killed
                if (heartImage[i] != null)
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
                        heartImage[i].enabled = true;
                    }
                    else
                    {
                        // Hide heart image
                        heartImage[i].enabled = false;
                    }
                }
                
            }
        }
        
    }
}
