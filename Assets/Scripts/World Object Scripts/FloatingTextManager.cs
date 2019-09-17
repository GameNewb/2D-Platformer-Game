using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    [Header("Message To Display")]
    [Space(1)]
    public string message;
    
    private bool interacting = false;

    private void ShowFloatingText()
    {
        var fText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        fText.SetActive(true);
        if (message != null)
        {
            fText.GetComponent<TextMeshPro>().text = message;
        }
    }

    public void ShowFloatingText(string textToShow)
    {
        var fText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        fText.SetActive(true);
        if (textToShow != null)
        {
            fText.GetComponent<TextMeshPro>().text = textToShow;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && floatingTextPrefab && !interacting)
        {
            StartCoroutine("ResetInteraction");
            ShowFloatingText();
        }
    }

    IEnumerator ResetInteraction()
    {
        interacting = true;
        yield return new WaitForSeconds(5f);

        interacting = false;
    }

}
