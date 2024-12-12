using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.TMP_InputField;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# 게임 컨트롤")]
    public float gameTime; // 현재 게임 시간
    public float maxGameTime = 2 * 10f; // 최대 게임 시간

    public bool isGameActive; // 게임 진행 여부
    public bool isGameRealEnd; // 게임 진짜 끝났는지 여부
    public bool isLevelUp; // 레벨업 중인지 여부    

    public int weaponCount = 0;  // 획득한 무기 개수
    public int gearCount = 0;    // 획득한 기어 개수
    public int maxItemCount = 1; // 최대 장착 가능한 무기/기어 개수
    public float nowTimeScale = 1f; // 현재 타임 스케일
    public float dieMsgDelay; // 죽는 메시지 나올 텀
    public int selectStageIdx; // 현재 선택된 스테이지
    private bool isGamestart; // 게임 시작된 상태인지(Ai 메시지 재활용 때문)
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 }; // 다음 레벨업에 필요한 경험치

    [Header("# 플레이 정보")]
    public int level; // 현재 레벨
    public int kill; // 처치한 적 수
    public int exp; // 현재 경험치
 
    [Header("# 참조")]
    public AiManager ai;
    public TypeControlManager typeControll;
    public PoolManager poolManager;
    public UIManager UIManager;
    public LevelUp uiLevelUp;
    public Player player;
    public Result result;
    
    [field: SerializeField] public PlayerData playerData { get; private set; } // 복사본
    [SerializeField] private PlayerData orignalPlayerData; // 원본
    [SerializeField] private GameObject enemyCleaner;

    [SerializeField] private float[] aiMsgShowTime = { 1.5f, 3f, 4f };
    private void Awake()
    {
        instance = this;
        selectStageIdx = -1;
        StartCoroutine(RandomStageIndex());
    }

    private void Start()
    {
        // 원본 훼손 안시키기 위함 (데이터 복사)
        if (playerData == null)
            playerData = Instantiate(orignalPlayerData);
        
        GameStart(playerData.characterId);
        StartCoroutine(AIMsgShowAndHide());
    }

    private void Update()
    {
        if (!isGameActive || !isGamestart)
            return;        

        // 게임 시간 계산
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory(); // 최대 시간 도달 시 승리 처리
        }      
    }
    // 이펙트 생성시키기
    public void GenerateEffect(int index, Transform parentTransform, Color? setColor = null)
    {            
        GameObject effect = poolManager.Get(PoolType.Effect, 0); // 플레이어 힐 이펙트
        effect.transform.parent = parentTransform;
        effect.transform.localPosition = Vector3.zero;
        if (setColor != null)
            effect.gameObject.GetComponent<SpriteRenderer>().color = setColor ?? Color.white;
    }

    // Ai 메세지 띄어주기
    public IEnumerator AIMsgShowAndHide()
    {
        yield return new WaitForSeconds(aiMsgShowTime[0]);

        ai.AppearAiImage(selectStageIdx);

        yield return new WaitForSeconds(aiMsgShowTime[1]);
        if (!isGamestart)
        {
            isGamestart = true;
            // 플레이어 기본 무기 부여
            uiLevelUp.FirstGiveWeapon(playerData.characterId);
            player.spawner.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(aiMsgShowTime[2]);

        ai.HideAi();
    }
    public IEnumerator RandomStageIndex()
    {
        List<int> availableIndices = new List<int>();

        // 현재 선택된 스테이지 인덱스를 제외하고 가능한 인덱스를 추가
        for (int i = 0; i < ai.alertMessages.Length; i++)
        {
            if (i != selectStageIdx)
            {
                availableIndices.Add(i);
            }
        }

        // 가능한 인덱스들 중 하나를 랜덤으로 선택
        selectStageIdx = availableIndices[Random.Range(0, availableIndices.Count)];

        yield break; // 코루틴을 바로 종료
    }
    // 게임 시작 설정
    public void GameStart(int playerId)
    {
        Resume();

        // BGM,SFX 설정
        SetupAudio();

        player.PlayerInit(playerId); // 플레이어 초기화
        player.gameObject.SetActive(true);

    }

    private static void SetupAudio()
    {
        if (TitleManager.playlistController == null)
            return;

        // BGM 설정
        if (TitleManager.playlistController.CurrentPlaylist.playlistName != "Game Bgm")
            MasterAudio.ChangePlaylistByName("Game Bgm");
        else
            MasterAudio.StartPlaylist("Game Bgm");        
    }

    // 게임 오버 처리
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    // 게임 오버 (코루틴)
    private IEnumerator GameOverRoutine()
    {
        isGameActive = false;
        isGameRealEnd = true;
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
        player.GetComponent<Animator>().SetFloat("Speed", 0f);
        isGameActive = false;
        isGameRealEnd = true;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        result.gameObject.SetActive(true);
        result.Win();

        MasterAudio.PlaylistsMuted = true; // 배경음악 종료        
        MasterAudio.PlaySound("Win");
    }


    // 경험치 획득 및 레벨업 처리
    public void GetExp(int getExp)
    {
        if (isGameActive)
        {
            exp += getExp;

            if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
            {
                // 게임 일시정지 후 레벨업 UI 띄우기
                Stop(); 
                level++;
                exp = 0;
                uiLevelUp.Show();
                isLevelUp = true;
            }
        }
    }

    // 게임 정지
    public void Stop()
    {
        if (!isGameRealEnd)
            isGameActive = false;

        Time.timeScale = 0;
    }

    // 게임 재개
    public void Resume()
    {
        if (!isGameRealEnd)            
            isGameActive = true;

        Time.timeScale = nowTimeScale;
    }
}
