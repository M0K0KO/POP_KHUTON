using System;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public GameObject loginPanel;
    public TMP_Text username;
    public TMP_Text password;

    public GameObject mainPanel;
    

    public void OnLoginClick()
    {
        loginPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    
    
}
