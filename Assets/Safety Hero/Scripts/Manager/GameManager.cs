using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# 게임 컨트롤")]
    public float gameTime; // 현재 게임 시간
    public float maxGameTime = 2 * 10f; // 최대 게임 시간
    public bool isLive; // 게임 진행 여부
    public int weaponCount = 0;  // 획득한 무기 개수
    public int gearCount = 0;    // 획득한 기어 개수
    public int maxItemCount = 1; // 최대 장착 가능한 무기/기어 개수
    public float dieMsgDelay;

    [Header("# 플레이어 정보")]
    public int playerId; // 플레이어 ID
    public float health; // 현재 체력
    public float maxHealth = 100; // 최대 체력
    public int level; // 현재 레벨
    public int kill; // 처치한 적 수
    public int exp; // 현재 경험치
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 }; // 다음 레벨업에 필요한 경험치

    [Header("# 참조")]
    public PoolManager pool;
    public LevelUp uiLevelUp;
    public Player player;
    public Result uiResult;
    public PlayerData playerData;
    public CurrentData currentData;
    public GameObject enemyCleaner;

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        if (playerData == null) 
            playerData = DataManager.instance.currentPlayerData;

        GameStart(playerData.characterId);
    }

    private void Update()
    {
        // 디버깅용 레벨업
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("레벨업");
            GetExp(nextExp[Mathf.Min(level, nextExp.Length - 1)]); // 최대 인덱스를 초과하지 않게
        }

        if (isLive)
        {
           
            // 시간 계산
            gameTime += Time.deltaTime;
            if (gameTime > maxGameTime)
            {
                gameTime = maxGameTime;
                GameVictory(); // 최대 시간 도달 시 승리 처리
            }
        }
        else
            return;
    }

    // 게임 시작 설정
    public void GameStart(int id)
    {      
        isLive = true;
        playerId = id; // 플레이어 아이디 세팅
        health = maxHealth * playerData.maxHpMult; // 플레이어 체력 세팅 
        uiLevelUp.Select(playerData.characterId); // 플레이어 기본 무기 부여

        player.PlayerInit(); // 플레이어 초기화
        player.gameObject.SetActive(true);

        AudioManager.instance.PlayBgm(true); // 배경음악 재생
    }

    // 게임 오버 처리
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    // 게임 오버 (코루틴)
    private IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(dieMsgDelay);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false); // 배경음악 종료
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose); // 패배 효과음 재생
    }

    // 게임 승리 처리
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    // 게임 승리 로직 (코루틴)
    private IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.instance.PlayBgm(false); // 배경음악 종료
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win); // 승리 효과음 재생
    }

    // 게임 재시작
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    // 게임 종료
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    // 경험치 획득 및 레벨업 처리
    public void GetExp(int getExp)
    {
        if (isLive)
        {
            exp += getExp;

            if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
            {
                level++;
                exp = 0;
                uiLevelUp.Show();
            }
        }
    }

    // 게임 정지
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    // 게임 재개
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
