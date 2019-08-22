using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    [Header("Message To Display")]
    [Space(1)]
    public string message;
    
    // Start is called before the first frame update
    void Start()
    {
        if (floatingTextPrefab)
        {
            ShowFloatingText();
        }
    }

    private void ShowFloatingText()
    {
        var fText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        fText.SetActive(true);
        if (message != null)
        {
            fText.GetComponent<TextMesh>().text = message;
        }
    }

    public void ShowFloatingText(string textToShow)
    {
        var fText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        fText.SetActive(true);
        if (textToShow != null)
        {
            fText.GetComponent<TextMesh>().text = textToShow;
        }
    }

}
