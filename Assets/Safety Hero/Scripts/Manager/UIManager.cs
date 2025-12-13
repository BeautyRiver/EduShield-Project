using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VInspector;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Foldout("UI Active 관리")]
    [Header("팝업 UI 모음")]
    public GameObject winUI; 
    public GameObject loseUI; 
    
    public LevelUp levelUp;
    public Result resultUI;

    public GameObject pauseUI;
    public GameObject optionUI;
    public GameObject rewardBoxOpenUI;
    public GameObject rewardBoxOkUI;

    private bool isPause;
    private bool isOption;
    [EndFoldout]

    [Foldout("Status Text % 관리")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI speedText;
    [EndFoldout]

    [SerializeField] private Image[] swapCoolDownImages;
    [SerializeField] private float fadeTime;

    private GlobalManager globalManager;
    private GameManager gameManager;

    private void Awake()
    {        
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        globalManager = GlobalManager.instance;
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (globalManager.gameState == GameState.LevelUp)
            return;
    } 

    private bool isOpening = false;

    // 보상 상자 UI 띄우기
    public void ShowRewardBox()
    {
        globalManager.ChangeGameState(GameState.Paused);
        rewardBoxOpenUI.SetActive(true);
        rewardBoxOpenUI.transform.localScale = Vector3.zero;
        rewardBoxOpenUI.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    // 보상 상자 열기 애니메이션 재생
    public void OpenRewardBox()
    {
        if (isOpening || rewardBoxOkUI.activeSelf)
            return;

        StartCoroutine(OpenRoutine());
    }    
    IEnumerator OpenRoutine() // 코루틴 함수
    {
        isOpening = true;

        Animator anim = rewardBoxOpenUI.GetComponentInChildren<Animator>();
        anim.SetBool("IsOpen", true);

        yield return new WaitForSecondsRealtime(0.8f);

        // 기다림이 끝나면 실행
        ShowRewardBoxOK();
    }

    // 보상 상자 확인 UI 띄우기
    public void ShowRewardBoxOK()
    {
        rewardBoxOkUI.SetActive(true);
        rewardBoxOkUI.transform.localScale = Vector3.zero;
        rewardBoxOkUI.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void RewardOkButton()
    {
        Animator anim = rewardBoxOpenUI.GetComponentInChildren<Animator>();
        anim.SetBool("IsOpen", false);

        MasterAudio.PlaySound("BtnClick");

        rewardBoxOkUI.SetActive(false);
        rewardBoxOpenUI.SetActive(false);

        isOpening = false;
        globalManager.ChangeGameState(GameState.Playing);
    }

    // 결과창(승리/패배) 띄우기
    public void ShowResult(bool isWin)
    {
        resultUI.gameObject.SetActive(true);
        if (isWin)
            resultUI.Win();
        else
            resultUI.Lose();
    }

    // ------------------------------------------
  
    public void TogglePauseUI()
    {
        if (isOption)
        {
            ToggleOptionUI();
            return;
        }

        if (!isPause)
        {
            globalManager.ChangeGameState(GameState.Paused);
            MasterAudio.PlaySound("BtnClick");
            isPause = true;
            UpdatePlayerStatusText();
        }
        else
        {
            globalManager.ChangeGameState(GameState.Playing);
            MasterAudio.PlaySound("BtnClick");
            isPause = false;
        }
        pauseUI.SetActive(isPause);
    }

    public void ToggleOptionUI()
    {
        if (!isOption && isPause)
        {
            MasterAudio.PlaySound("BtnClick");
            isOption = true;
        }
        else
        {
            MasterAudio.PlaySound("BtnClick");
            isOption = false;
        }
        optionUI.SetActive(isOption);
    }

    private void UpdatePlayerStatusText()
    {
        if (gameManager.playerData == null) return;

        hpText.text = (gameManager.playerData.maxHpMult * 100f).ToString() + "%";
        damageText.text = (gameManager.playerData.damageMult * 100f).ToString() + "%";
        attackSpeedText.text = (gameManager.playerData.attackSpeedMult * 100f).ToString() + "%";
        attackRangeText.text = (gameManager.playerData.attackRangeMult * 100f).ToString() + "%";
        speedText.text = (gameManager.playerData.speedMult * 100f).ToString() + "%";
    }

    // (기존 Win/Lose 함수는 ShowResult로 대체 가능하지만, 호환성을 위해 남겨둠)
    public void Lose() => loseUI.SetActive(true);
    public void Win() => winUI.SetActive(true);

    public void GameRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoTitle()
    {
        MasterAudio.PlaylistsMuted = false;
        globalManager.ChangeGameState(GameState.Playing);
        LoadingSceneController.LoadScene("Title Scene");
    }    
}