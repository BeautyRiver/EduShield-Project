using DarkTonic.MasterAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using VInspector;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# 게임 컨트롤")]
    public List<int> nextExp = new List<int>();
    public float gameTime;
    public float maxGameTime;

    public int curretWeaponCount = 0;
    public int gearCount = 0;
    public int maxItemCount = 1;
    public float nowTimeScale = 1f;
    public float gameOverDelay;

    [Header("플레이어 참조")]
    public PlayerInGame player;

    [Foldout("# 참조")]
    private GlobalManager globalManager;
    private HUDManager hud;
    public SpawnManager spawner;
    [EndFoldout]

    [field: SerializeField] public PlayerData playerData { get; private set; }
    [SerializeField] private PlayerData orignalPlayerData;
    [SerializeField] private GameObject enemyCleaner;

    private void Awake()
    {
        instance = this;

        if (playerData == null)
            playerData = Instantiate(orignalPlayerData);
    }

    private void Start()
    {
        globalManager = GlobalManager.instance;
        hud = HUDManager.instance;
        GameStart();
    }

    private void Update()
    {
        if (globalManager.gameState != GameState.Playing)
            return;

        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            globalManager.ChangeGameState(GameState.Victory);
        }
        hud.UpdateTime(gameTime, maxGameTime);
    }

    public void GameStart()
    {
        SetupAudio();
        player.PlayerInit(playerData);
        player.gameObject.SetActive(true);

        InitFirstWeaponSelection(playerData.characterId);

        spawner.gameObject.SetActive(true);

        Box.openCount = 0;        

        globalManager.ChangeGameState(GameState.Playing);
        globalManager.ChangePlayerState(PlayerState.FreeMove);
    }

    private static void SetupAudio()
    {
        MasterAudio.StartPlaylist("Game Bgm");
    }


    // 초기 무기 선택
    public void InitFirstWeaponSelection(int characterId)
    {
        UIManager.instance.levelUp.FirstGiveWeapon(characterId);
    }

    [Button]
    public void SetNextExp()
    {
        nextExp.Clear();
        float currentMaxExp = 7f;
        float growthRate = 1.4f;
        for (int i = 0; i < 25; i++)
        {
            nextExp.Add(Mathf.RoundToInt(currentMaxExp));
            currentMaxExp *= growthRate;
            growthRate = Mathf.Max(1.01f, growthRate - 0.0135f);
        }
    }

    public IEnumerator GameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        // UIManager에게 패배 UI 요청
        UIManager.instance.ShowResult(false);

        MasterAudio.PlaylistsMuted = true;
        MasterAudio.PlaySound("Lose");
    }

    public IEnumerator GameVictoryRoutine()
    {
        player.GetComponent<Animator>().SetFloat("Speed", 0f);
        enemyCleaner.SetActive(true); // 적 청소는 게임 로직이므로 여기서 유지
        yield return new WaitForSeconds(0.5f);

        // UIManager에게 승리 UI 요청
        UIManager.instance.ShowResult(true);

        MasterAudio.PlaylistsMuted = true;
        MasterAudio.PlaySound("Win");
    }
}