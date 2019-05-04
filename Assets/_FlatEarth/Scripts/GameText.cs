using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameText : MonoBehaviour
{
    [SerializeField] private Text frontText;
    [SerializeField] private Text backText;

    public void SetText(string text)
    {
        frontText.text = text;
        backText.text = text;
    }

    public void Show()
    {
        frontText.gameObject.SetActive(true);
        backText.gameObject.SetActive(true);
    }

    public void Hide() 
    {
        frontText.gameObject.SetActive(false);
        backText.gameObject.SetActive(false);
    }
}
