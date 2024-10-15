using DarkTonic.MasterAudio;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject[] titles;
    public GameObject uiOption;

    public Image fadeImage;
    public Image[] swapCoolDownImages;
    [SerializeField]
    private float fadeTime;

    private GameManager gm;

    private void Start()
    {
        fadeImage.gameObject.SetActive(true);
        gm = GameManager.instance;
        fadeImage.DOFade(0, fadeTime).OnComplete(() => fadeImage.gameObject.SetActive(false));
    }
    private void Update()
    {
        if (!gm.isLive)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!uiOption.activeSelf)
            {
                MasterAudio.PlaySound("BtnClick");
                uiOption.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                MasterAudio.PlaySound("BtnClick");
                uiOption.SetActive(false);
                Time.timeScale = gm.nowTimeScale;
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
        MasterAudio.PlaylistsMuted = false; // 배경음악 On        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoTitle()
    {
        MasterAudio.PlaylistsMuted = false; // 배경음악 On         
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
