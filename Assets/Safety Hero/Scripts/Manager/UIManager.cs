using DarkTonic.MasterAudio;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private bool isPause;
    private bool isOption;
    [EndFoldout]

    [Foldout("Status Text % 관리")]
    [SerializeField] private TextMeshProUGUI hpText; // 체력
    [SerializeField] private TextMeshProUGUI damageText; // 데미지
    [SerializeField] private TextMeshProUGUI attackSpeedText; // 공격 속도    
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI speedText; // 이동 속도
    [EndFoldout]

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseScreen();
        }
    }
    // 일시정지 화면 On/Off
    public void TogglePauseScreen()
    {
        if (isOption)
        {
            ToggleOptionScreen();
            return;
        }

        if (!isPause)
        {
            MasterAudio.PlaySound("BtnClick");
            BlackWindowFadeIn();
            isPause = true;            
            gm.Stop();
            pauseFirstSelectedButton.Select();
            UpdatePlayerStatusText();
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

    private void UpdatePlayerStatusText()
    {
        hpText.text = (gm.playerData.maxHpMult * 100f).ToString() + "%";
        damageText.text = (gm.playerData.damageMult * 100f).ToString() + "%";
        attackSpeedText.text = (gm.playerData.attackSpeedMult * 100f).ToString() + "%";
        attackRangeText.text = (gm.playerData.attackRangeMult * 100f).ToString() + "%";
        speedText.text = (gm.playerData.speedMult * 100f).ToString() + "%";
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

}
