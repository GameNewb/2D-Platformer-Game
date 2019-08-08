using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersistence : MonoBehaviour
{
    int initialSceneIndex;

    // Singleton Class
    private void Awake()
    {
        int numScenePersist = FindObjectsOfType<ScenePersistence>().Length;

        // Check index
        if (numScenePersist > 1)
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
        initialSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex != initialSceneIndex)
        {
            Destroy(gameObject);
        }
    }

}
