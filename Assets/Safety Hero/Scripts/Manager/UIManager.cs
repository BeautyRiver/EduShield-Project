using DarkTonic.MasterAudio;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    public GameObject[] titles;
    public GameObject pauseUi;
    public Image blackWindow; // 레벨업, esc 뒤의 배경 검게
    public Image[] swapCoolDownImages;
    [SerializeField]
    private float fadeTime;
    private GameManager gm;

    private void Start()
    {        
        gm = GameManager.instance;
    }

    // 일시정지 화면 On/Off
    public void TogglePauseScreen()
    {
        if (!pauseUi.activeSelf)
        {
            //ButtonKeyBoardSelector.instance.InitializeNavigation(pauseUi); // 키보드로 선택가능한 버튼들 할당
            BlackWindowFadeIn();
            MasterAudio.PlaySound("BtnClick");
            pauseUi.SetActive(true);
            gm.Stop();
        }
        else
        {
            MasterAudio.PlaySound("BtnClick");
            BlackWindowFadeOut();
            pauseUi.SetActive(false);
            gm.Resume();
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
        blackWindow.gameObject.SetActive(true);
    }
    // 검은 배경 Off
    public void BlackWindowFadeOut()
    {
        blackWindow.gameObject.SetActive(false);
    }
}
