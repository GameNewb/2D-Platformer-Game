using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int gold = 0;
    [SerializeField] Text liveText;
    [SerializeField] Text goldText;

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

    // Start is called before the first frame update
    void Start()
    {
        liveText.text = playerLives.ToString();
        goldText.text = gold.ToString();
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            DealDamage();
        }
        else
        {
            ResetGameSession();
        }
    }

    public void ProcessScore(int pointsToAdd)
    {
        gold += pointsToAdd;
        goldText.text = gold.ToString();
    }

    private void DealDamage()
    {
        playerLives--;
        liveText.text = playerLives.ToString();
        goldText.text = gold.ToString();

        // Reload current scene
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void ResetGameSession()
    {
        // Main Menu
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
