using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameText : MonoBehaviour
{
#pragma warning disable
    [SerializeField] private Text frontText;
    [SerializeField] private Text backText;
    [SerializeField] private GameObject logo;
#pragma warning enable

    private void Start()
    {
        GameManager.OnRoundEnd += (Player winner) => { 
            SetText($"{(winner == null ? "Earth" : winner.playerName)} won");
            logo.SetActive(true);
            Show();
        };
        GameManager.OnRoundStart += () => 
        {
            SetText("GO!");
            Eitrum.Engine.Core.Timer.Once(0.5f, Hide);
        };
        GameManager.OnCountDown += (int count) => { SetText(count.ToString()); };
        GameManager.OnRestart += () => { 
            SetText(3.ToString()); 
            Show(); 
            logo.SetActive(false);
            FMODUnity.RuntimeManager.PlayOneShot("event:/countdown");
            };
    }

    private void SetText(string text)
    {
        frontText.text = text;
        backText.text = text;
    }

    private void Show()
    {
        frontText.gameObject.SetActive(true);
        backText.gameObject.SetActive(true);
    }

    private void Hide() 
    {
        frontText.gameObject.SetActive(false);
        backText.gameObject.SetActive(false);
        logo.SetActive(false);
    }
}
