using DarkTonic.MasterAudio;
using System;
using System.Collections;
using UnityEngine;
using VInspector;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# 게임 컨트롤")]
    public float gameTime; // 현재 게임 시간
    public float maxGameTime; // 최대 게임 시간


    public int weaponCount = 0;  // 획득한 무기 개수
    public int gearCount = 0;    // 획득한 기어 개수
    public int maxItemCount = 1; // 최대 장착 가능한 무기/기어 개수
    public float nowTimeScale = 1f; // 현재 타임 스케일
    public float gameOverDelay; // 죽는 메시지 나올 텀
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 }; // 다음 레벨업에 필요한 경험치    

    [Header("# 플레이어 정보")]
    public int playerLevel; // 현재 레벨
    public int playerKill; // 처치한 적 수
    public int playerExp; // 현재 경험치
    public int gold; // 현재 골드

    [Header("플레이어 참조")]
    public PlayerInGame player;
    private PlayerInputController playerInputController;

    [Foldout("# 참조")]
    private GlobalManager glM;
    private HUDManager hud;
    public SpawnManager spawner;
    public LevelUp uiLevelUp;
    public Result result;
    [EndFoldout]
    [field: SerializeField] public PlayerData playerData { get; private set; } // 복사본
    [SerializeField] private PlayerData orignalPlayerData; // 원본
    [SerializeField] private GameObject enemyCleaner;
    private void Awake()
    {
        instance = this;

        // 원본 훼손 안시키기 위함 (데이터 복사)
        if (playerData == null)
            playerData = Instantiate(orignalPlayerData);

        playerInputController = player.GetComponent<PlayerInputController>();
    }

    private void Start()
    {
        glM = GlobalManager.instance;
        hud = HUDManager.instance;
        ChangeState(GameState.Ready);
        GameStart();
    }
      
    private void Update()
    {
        if (glM.gameState != GameState.Playing)
            return;

        // 게임 시간 계산
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            ChangeState(GameState.Victory); // 상태 변경으로 승리 처리
        }
        hud.UpdateTime(gameTime, maxGameTime);
    }

    // 상태 변경 처리
    public void ChangeState(GameState newState)
    {
        if (glM.gameState == newState) return;

        glM.gameState = newState;

        switch (glM.gameState)
        {
            case GameState.Ready:
                playerInputController.SwitchActionMap("Empty"); // 입력 비활성화
                Time.timeScale = nowTimeScale; // 여기서 nowTimeScale을 사용해도 됨                
                break;

            case GameState.Playing:
                playerInputController.SwitchActionMap("InGame"); // 인게임 움직임 활성화
                Time.timeScale = nowTimeScale;
                break;

            case GameState.Paused:
                playerInputController.SwitchActionMap("UI"); // UI 입력 활성화
                Time.timeScale = 0f;
                break;

            case GameState.LevelUp:
                playerInputController.SwitchActionMap("UI"); // UI 입력 활성화
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                playerInputController.SwitchActionMap("Empty"); // 입력 비활성화
                Time.timeScale = 0f;
                StartCoroutine(GameOverRoutine());
                break;

            case GameState.Victory:
                playerInputController.SwitchActionMap("Empty"); // 입력 비활성화
                Time.timeScale = 1f; // 승리 연출을 위해 시간을 다시 흐르게 할 수도 있음
                StartCoroutine(GameVictoryRoutine());
                break;
        }

    }

    // 게임 시작 설정
    public void GameStart()
    {        
        SetupAudio(); // BGM,SFX 설정
        player.PlayerInit(playerData); // 플레이어 초기화
        player.gameObject.SetActive(true);                
        uiLevelUp.FirstGiveWeapon(playerData.characterId); // 플레이어 기본 무기 부여
        spawner.gameObject.SetActive(true);

        ChangeState(GameState.Playing); // 이제부터 Playing 상태
    }

    public void PauseGame()
    {
        if (glM.gameState == GameState.Playing)
            ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (glM.gameState == GameState.Paused)
            ChangeState(GameState.Playing);
    }

    public void GetExp(int getExp)
    {
        if (glM.gameState != GameState.Playing) return;

        playerExp += getExp;
        int maxExp = nextExp[Mathf.Min(playerLevel, nextExp.Length - 1)];

        if (playerExp >= maxExp)
        {
            hud.UpdateExp(maxExp, maxExp, LevelUp);
        }

        else
        {
            hud.UpdateExp(playerExp, maxExp);
        }

    }

    private void LevelUp()
    {
        playerLevel++;

        // 경험치 이월
        int maxExp = nextExp[Mathf.Min(playerLevel - 1, nextExp.Length - 1)];
        playerExp = playerExp - maxExp;

        ChangeState(GameState.LevelUp); // 레벨업 상태로 변경
        uiLevelUp.Show();

        maxExp = nextExp[Mathf.Min(playerLevel, nextExp.Length - 1)];
        hud.UpdateExp(playerExp, maxExp);
    }


    // LevelUp UI가 닫힐 때 호출
    public void EndLevelUp()
    {
        ChangeState(GameState.Playing); // 다시 플레이 상태로 복귀
    }

    private static void SetupAudio()
    {
        MasterAudio.StartPlaylist("Game Bgm");        
    } 

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay); // TimeScale이 0이므로 Realtime 사용
        result.gameObject.SetActive(true);
        result.Lose();
        MasterAudio.PlaylistsMuted = true;
        MasterAudio.PlaySound("Lose");
    }

    private IEnumerator GameVictoryRoutine()
    {
        player.GetComponent<Animator>().SetFloat("Speed", 0f);
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        result.gameObject.SetActive(true);
        result.Win();
        MasterAudio.PlaylistsMuted = true;
        MasterAudio.PlaySound("Win");
    }

    // 플레이어 처치 수 증가
    public void IncreasePlayerKill()
    {
        playerKill++;
        hud.UpdateKill(playerKill);
    }

    public void IncreaseGold(int amount)
    {
        gold += amount;
        hud.UpdateGold(gold);
    }
}
