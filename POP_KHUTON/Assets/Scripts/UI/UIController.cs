using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonSound;
    
    public GameObject loginPanel;
    public GameObject loginFrame;
    public TMP_Text username;
    public TMP_Text password;

    public GameObject signUpPanel;
    public GameObject signUpFrame;
    public TMP_Text signUpNickname;
    public TMP_Text signUpUsername;
    public TMP_Text signUpPassword;

    public GameObject mainPanel;
    
    

    private void Awake()
    {
        loginPanel.SetActive(true);
        mainPanel.SetActive(false);
        signUpPanel.SetActive(false);
        signUpPanel.GetComponent<CanvasGroup>().alpha = 0;
    }


    public void OnLoginClick()
    {
        audioSource.PlayOneShot(buttonSound);
        mainPanel.SetActive(true);
        
        loginFrame.GetComponent<RectTransform>().DOScale(1.4f, 0.3f);
        loginPanel.GetComponent<CanvasGroup>().DOFade(0, 0.4f).OnComplete(()=>loginPanel.SetActive(false));

    }

    public void OnSignUpClick()
    {
        audioSource.PlayOneShot(buttonSound);
        loginFrame.GetComponent<CanvasGroup>().blocksRaycasts = false;
        loginFrame.GetComponent<CanvasGroup>().DOFade(0, 0.4f).OnComplete((() => 
        {
            loginFrame.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }));
        signUpPanel.SetActive(true);
        signUpPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        signUpPanel.GetComponent<CanvasGroup>().DOFade(1, 0.4f).OnComplete((() =>
        {
            signUpPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }));
        
    }

    public void OnSignUpCompleteClick()
    {
        audioSource.PlayOneShot(buttonSound);
        loginFrame.SetActive(true);
        loginFrame.GetComponent<CanvasGroup>().alpha = 1;
        loginFrame.GetComponent<CanvasGroup>().blocksRaycasts = false;
        signUpPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        signUpPanel.GetComponent<CanvasGroup>().DOFade(0, 0.4f).OnComplete((() =>
        {
            loginFrame.GetComponent<CanvasGroup>().blocksRaycasts = true;
            signUpPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            signUpPanel.SetActive(false);
        }));
    }
}
