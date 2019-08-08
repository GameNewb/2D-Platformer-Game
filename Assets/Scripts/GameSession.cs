using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Text gemText;
    [SerializeField] int gem = 0;

    // Singleton Class
    private void Awake()
    {
        int numberOfGameSessions = FindObjectsOfType<GameSession>().Length;

        if (numberOfGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        gemText.text = gem.ToString() + "x";
    }

    public void ProcessPlayerDeath()
    {
        if (healthSystem != null)
        {
            if (healthSystem.currentHealth > 1)
            {
                DealDamage();
            }
            else
            {
                ResetGameSession();
            }
        }
    }

    public void ProcessGold(int pointsToAdd)
    {
        gem += pointsToAdd;
        gemText.text = gem.ToString() + "x";
    }

    private void DealDamage()
    {
        healthSystem.currentHealth--;
    }

    private void ResetGameSession()
    {
        // Main Menu
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
