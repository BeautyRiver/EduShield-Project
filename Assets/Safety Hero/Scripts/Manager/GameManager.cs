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
    public float nowTimeScale = 1f; // 현재 타임 스케일
    public float dieMsgDelay; // 죽는 메시지 나올 텀
    public int selectStageIdx; // 현재 선택된 스테이지
    private bool isGamestart; // 게임 시작된 상태인지(Ai 메시지 재활용 때문)
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 }; // 다음 레벨업에 필요한 경험치

    [Header("# 플레이어 정보")]
    public int playerId; // 플레이어 ID
    public float health; // 현재 체력
    public float maxHealth = 100; // 최대 체력
    public int level; // 현재 레벨
    public int kill; // 처치한 적 수
    public int exp; // 현재 경험치
 
    [Header("# 참조")]
    public AiManager ai;
    public EquipmentManager equipment;
    public TypeControlManager typeControll;
    public PoolManager pool;

    public LevelUp uiLevelUp;
    public Player player;
    public Result result;
    public PlayerData playerData;
    public GameObject enemyCleaner;

    [SerializeField] private float[] aiMsgShowTime = { 1.5f, 3f, 4f };
    private void Awake()
    {
        instance = this;

        selectStageIdx = -1;
        StartCoroutine(RandomStageIndex());
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
        if (!isLive || !isGamestart)
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
        GameObject effect = pool.Get(PoolManager.PoolType.Effect, index); // 힐 이펙트
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
            uiLevelUp.FirstGiveWeapon(playerData.characterId); // 플레이어 기본 무기 부여
            player.spawner.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(aiMsgShowTime[2]);

        ai.HideAi();
    }
    public IEnumerator RandomStageIndex()
    {
        int ranIdx;
        do
        {
            yield return null;
            ranIdx = Random.Range(0, ai.alertMessages.Length);
        } while (ranIdx == selectStageIdx);  // 같은 값일 때만 반복

        selectStageIdx = ranIdx;
    }
    // 게임 시작 설정
    public void GameStart(int playerId)
    {
        Resume();

        // BGM,SFX 설정
        /*if (TitleManager.playlistController.CurrentPlaylist.playlistName != "Game Bgm")
            MasterAudio.ChangePlaylistByName("Game Bgm");
        else
            MasterAudio.StartPlaylist("Game Bgm");*/

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
        player.GetComponent<Animator>().SetFloat("Speed", 0f);
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
        Time.timeScale = nowTimeScale;
    }
}
