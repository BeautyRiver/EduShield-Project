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
using VInspector;

public class UIManager : MonoBehaviour
{
    [Foldout("UI Active 관리")]
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject optionUI;
    [EndFoldout]

    private bool isPause;
    private bool isOption;

    [Foldout("처음 선택되는 버튼들")]
    [SerializeField] private Selectable pauseFirstSelectedButton; // 일시정지 씬에서 처음 선택되는 버튼    
    [SerializeField] private Selectable optionFirstSelectedButton; // 옵션 씬에서 처음 선택되는 버튼
    [EndFoldout]

    [SerializeField] private Image blackWindow; // 레벨업, esc 뒤의 배경 검게
    [SerializeField] private Image[] swapCoolDownImages;

    [SerializeField] private float fadeTime;

    private GameManager gm;

    
    private void Start()
    {        
        gm = GameManager.instance;
    }

    // 일시정지 화면 On/Off
    public void TogglePauseScreen()
    {
        if (!isPause)
        {
            MasterAudio.PlaySound("BtnClick");
            BlackWindowFadeIn();
            isPause = true;            
            gm.Stop();
            pauseFirstSelectedButton.Select();
        }
        else
        {
            MasterAudio.PlaySound("BtnClick");
            BlackWindowFadeOut();
            isPause = false;
            gm.Resume();
        }
        pauseUI.SetActive(isPause);
    }
    
    public void ToggleOptionScreen()
    {
        if (!isOption && isPause)
        {
            MasterAudio.PlaySound("BtnClick");
            isOption = true;
            optionFirstSelectedButton.Select();
        }
        else
        {
            MasterAudio.PlaySound("BtnClick");
            isOption = false;
            pauseFirstSelectedButton.Select();
        }
        optionUI.SetActive(isOption);
    }

    public void Lose()
    {
        loseUI.SetActive(true);
    }

    public void Win()
    {
        winUI.SetActive(true);
    }

    // 게임 재시작
    public void GameRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 타이틀로 이동
    public void GoTitle()
    {
        MasterAudio.PlaylistsMuted = false; // 배경음악 On         
        gm.Resume();
        LoadingSceneController.LoadScene("Title Scene");
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

    // 게임 종료
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
