using DarkTonic.MasterAudio;
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
    public int selectStageIdx;
    private bool isGamestart;

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
    public AiManager aiManager;
    public LevelUp uiLevelUp;
    public Player player;
    public Result result;
    public PlayerData playerData;
    public CurrentData currentData;
    public GameObject enemyCleaner;
    
    [SerializeField] private float[] aiMsgShowTime = { 1.5f, 3f, 4f };
    private void Awake()
    {
        selectStageIdx = -1;
        instance = this;
        RandomStageIndex();
    }

    private void Start()
    {
        if (playerData == null) 
            playerData = DataManager.instance.CurrentPlayerData;

        GameStart(playerData.characterId);
        StartCoroutine(AIMsgShowAndHide());        
    }

    private void Update()
    {        
        // 디버깅용 레벨업
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("레벨업");
            GetExp(nextExp[Mathf.Min(level, nextExp.Length - 1)]); // 최대 인덱스를 초과하지 않게
        }

        if (isLive && isGamestart)
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

    // Ai 메세지 띄어주기
    public IEnumerator AIMsgShowAndHide()
    {
        yield return new WaitForSeconds(aiMsgShowTime[0]);

        aiManager.AppearAiImage(selectStageIdx);

        yield return new WaitForSeconds(aiMsgShowTime[1]);
        if (!isGamestart)
        {
            uiLevelUp.Select(playerData.characterId); // 플레이어 기본 무기 부여
            player.spawner.gameObject.SetActive(true);
            isGamestart = true;
        }
        yield return new WaitForSeconds(aiMsgShowTime[2]);

        aiManager.HideAi();
    }
    public void RandomStageIndex()
    {
        int ranIdx;
        do
        {
            ranIdx = Random.Range(0, aiManager.alertMessages.Length);
        } while (ranIdx == selectStageIdx);  // 같은 값일 때만 반복

        selectStageIdx = ranIdx;
    }
    // 게임 시작 설정
    public void GameStart(int playerId)
    {
        Resume();
        if (TitleManager.playlistController.CurrentPlaylist.playlistName != "Game Bgm")
            MasterAudio.ChangePlaylistByName("Game Bgm");
        else
            MasterAudio.StartPlaylist("Game Bgm");

        MasterAudio.PlaylistsMuted = false;
        this.playerId = playerId; // 플레이어 아이디 세팅
        health = maxHealth * playerData.maxHpMult; // 플레이어 체력 세팅 

        player.PlayerInit(); // 플레이어 초기화
        player.gameObject.SetActive(true);

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

        result.gameObject.SetActive(true);
        result.Lose();
        Stop();

        MasterAudio.PlaylistsMuted = true; // 배경음악 종료        
        MasterAudio.PlaySound("Lose");
    }

    // 게임 승리 처리
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    // 게임 승리 로직 (코루틴)
    private IEnumerator GameVictoryRoutine()
    {
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        isLive = false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        result.gameObject.SetActive(true);
        result.Win();
        isLive = false;

        MasterAudio.PlaylistsMuted = true; // 배경음악 종료        
        MasterAudio.PlaySound("Win");
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
