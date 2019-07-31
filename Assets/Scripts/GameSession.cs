using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] HealthSystem healthSystem;

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
    }

    public void ProcessPlayerDeath()
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
