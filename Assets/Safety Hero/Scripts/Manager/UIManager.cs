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

    public Image startFadeImage; // 처음 페이드인 아웃 이미지
    public Image blackWindow; // 레벨업, esc 뒤의 배경 검게
    public Image[] swapCoolDownImages;
    [SerializeField]
    private float fadeTime;
    private GameManager gm;

    private void Start()
    {
        startFadeImage.gameObject.SetActive(true);
        gm = GameManager.instance;
        startFadeImage.DOFade(0, fadeTime).OnComplete(() => startFadeImage.gameObject.SetActive(false));
    }
    private void Update()
    {
        if (!gm.isGameActive)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!uiOption.activeSelf)
            {
                BlackWindowFadeIn();
                MasterAudio.PlaySound("BtnClick");
                uiOption.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                BlackWindowFadeaOut();
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

    // 검은 배경 On
    public void BlackWindowFadeIn()
    {
        blackWindow.DOFade(0.8f, 0.25f).SetUpdate(true); 
    }
    // 검은 배경 Off
    public void BlackWindowFadeaOut()
    {
        blackWindow.DOFade(0f, 0.25f).SetUpdate(true); 
    }
}
