using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    public GameObject panelObject;
    public Animator transitionAnimator;

    public Vector2 playerPosition;
    public PlayerPosition playerStorage;

    private Scene currentLevel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerStorage.initialValue = playerPosition;
            currentLevel = SceneManager.GetActiveScene();
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        transitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1.2f);

       // panelObject.GetComponent<Image>().enabled = false;
        SceneManager.LoadScene(nextLevel);
    }
}
