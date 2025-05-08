using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class LoginController : MonoBehaviour
{
    public GameObject loginPanel;
    public TMP_Text username;
    public TMP_Text password;

    public GameObject mainPanel;

    private void Awake()
    {
        loginPanel.SetActive(true);
        mainPanel.SetActive(false);
    }


    public void OnLoginClick()
    {
        loginPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete((() =>
        {
            loginPanel.SetActive(false);
            mainPanel.SetActive(true);
        }));
        
    }
    
    
}
