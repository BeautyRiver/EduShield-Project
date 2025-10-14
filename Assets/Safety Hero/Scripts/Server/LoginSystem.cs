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

        bool success = await FirebaseManager.Instance.CreateAccount(e, p);

        if (success)
        {
            Debug.Log("회원가입 성공! 로그인도 자동으로 완료되었습니다.");
            alert.color = Color.black;
            alert.text = "회원가입 성공!";
            SceneManager.LoadScene("Title Scene"); 
        }
        else
        {
            Debug.Log("회원가입에 실패했습니다. 이메일 형식을 확인하거나 다른 이메일을 사용하세요.");
            alert.color = Color.red;
            alert.text = "회원가입에 실패했습니다.";
        }
    }

    public async void LogIn()
    {
        string e = email.text;
        string p = password.text;

        bool success = await FirebaseManager.Instance.Login(e, p);

        if (success)
        {
            Debug.Log("로그인 성공!");
            alert.color = Color.black;
            alert.text = "로그인 성공!";
            SceneManager.LoadScene("Title Scene");
        }
        else
        {
            Debug.Log("로그인에 실패했습니다. 이메일 또는 비밀번호를 확인하세요.");
            alert.color = Color.red;
            alert.text = "로그인에 실패했습니다.";
        }
    }

    public void Logout()
    {
        FirebaseManager.Instance.Logout();
        SceneManager.LoadScene("Login"); 
    }
}
