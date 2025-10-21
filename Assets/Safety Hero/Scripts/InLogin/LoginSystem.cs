using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSystem : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TextMeshProUGUI alert;

    public async void Create()
    {
        string e = email.text;
        string p = password.text;

        string errorMessage = await FirebaseManager.Instance.CreateAccount(e, p);

        if (string.IsNullOrEmpty(errorMessage))
        {
            Debug.Log("회원가입 성공! 로그인도 자동으로 완료되었습니다.");
            alert.color = Color.black;
            alert.text = "회원가입 성공!";
            SceneManager.LoadScene("Title Scene"); 
        }
        else
        {
            alert.color = Color.red;
            alert.text = errorMessage;
        }
    }

    public async void LogIn()
    {
        string e = email.text;
        string p = password.text;

        string errorMessage = await FirebaseManager.Instance.CreateAccount(e, p);

        if (string.IsNullOrEmpty(errorMessage))
        {
            Debug.Log("로그인 성공!");
            alert.color = Color.black;
            alert.text = "로그인 성공!";
            SceneManager.LoadScene("Title Scene");
        }
        else
        {
            alert.color = Color.red;
            alert.text = errorMessage;
        }
    }

    public void Logout()
    {
        FirebaseManager.Instance.Logout();
        SceneManager.LoadScene("Login"); 
    }
}
