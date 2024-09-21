using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject[] titles;
    public Transform uiOption;
    public Image fadeImage;
    [SerializeField] float fadeTime;
    private GameManager gm;
    private void Start()
    {
        gm = GameManager.instance;
        fadeImage.DOFade(0, fadeTime).OnComplete(()=> fadeImage.gameObject.SetActive(false));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiOption.localScale != Vector3.one)
            {
                MasterAudio.PlaySound("BtnClick");
                uiOption.localScale = Vector3.one;
                gm.Stop();
            }
            else
            {
                MasterAudio.PlaySound("BtnClick");
                uiOption.localScale = Vector3.zero;
                gm.Resume();
            }
        }
    }
        public void Lose()
    {
        titles[0].SetActive(true);
    }

    public void Win()
    {
        titles[1].SetActive(true);
    }

    // 게임 재시작
    public void GameRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoTitle()
    {
       gm.Resume();
        LoadingSceneController.LoadScene("Title Scene");
    }
    // 게임 종료
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
