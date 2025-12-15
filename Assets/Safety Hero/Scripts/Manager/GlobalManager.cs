using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    FreeMove,
    InUI
}
public enum GameState
{
    Lobby,      // 로비 상태
    Ready,      // 게임 시작 전 준비 상태
    Playing,    // 게임 플레이 중
    Paused,     // 일시정지
    LevelUp,    // 레벨업 선택 중
    GameOver,   // 게임 오버
    Victory     // 게임 승리
}

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager instance;
    [SerializeField] private PlayerInput playerInput;
    public float nowTimeScale = 1;
    public PlayerState playerState;
    public GameState gameState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ChangeGameState(GameState newState)
    {
        if (gameState == newState) return;

        gameState = newState;

        switch (gameState)
        {
            case GameState.Ready:
                SwitchActionMap("Empty"); // 입력 비활성화
                Time.timeScale = nowTimeScale; // 여기서 nowTimeScale을 사용해도 됨                
                break;

            case GameState.Playing:
                ChangePlayerState(PlayerState.FreeMove);
                SwitchActionMap("InGame"); // 인게임 움직임 활성화
                Time.timeScale = nowTimeScale;
                break;

            case GameState.Paused:
                ChangePlayerState(PlayerState.InUI);
                SwitchActionMap("UI"); // UI 입력 활성화
                Time.timeScale = 0f;
                break;

            case GameState.LevelUp:
                ChangePlayerState(PlayerState.InUI);
                SwitchActionMap("UI"); // UI 입력 활성화
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                ChangePlayerState(PlayerState.InUI);
                SwitchActionMap("Empty"); // 입력 비활성화
                Time.timeScale = 0f;
                StartCoroutine(GameManager.instance.GameOverRoutine());
                break;

            case GameState.Victory:
                ChangePlayerState(PlayerState.InUI);
                SwitchActionMap("Empty"); // 입력 비활성화
                Time.timeScale = nowTimeScale; // 승리 연출을 위해 시간을 다시 흐르게 할 수도 있음
                StartCoroutine(GameManager.instance.GameVictoryRoutine());
                break;
        }
    }

    public void ChangePlayerState(PlayerState newState)
    {
        //if (newState == PlayerState.InUI)
        //{
        //    StopMovement();
        //}

        playerState = newState;
        switch (playerState)
        {
            case PlayerState.FreeMove:
                // TODO: 씬에 따라 "InLobby" 또는 "InGame" 맵을 선택해야 함
                SwitchActionMap("InLobby");
                break;
            case PlayerState.InUI:
                SwitchActionMap("UI");
                break;
        }
    }

    public void SwitchActionMap(string mapName)
    {
        // [안전장치 1] 만약 playerInput 변수가 연결이 안 되어 있다면?
        if (playerInput == null)
        {
            // GameManager가 알고 있는 플레이어에게서 컴포넌트를 직접 찾아옵니다.
            if (GameManager.instance != null && GameManager.instance.player != null)
            {
                playerInput = GameManager.instance.player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            }
        }

        // [안전장치 2] 그래도 null이면 에러를 내지 않고 리턴 (다음 프레임에 시도되거나 무시됨)
        if (playerInput == null)
        {
            // Debug.LogWarning("PlayerInput을 찾을 수 없어 액션 맵 전환을 건너뜁니다.");
            return;
        }

        // [안전장치 3] 컴포넌트는 있는데 꺼져있다면 켜주기
        if (!playerInput.enabled)
        {
            playerInput.enabled = true;
        }

        // 안전하게 전환
        playerInput.SwitchCurrentActionMap(mapName);
    }

    public void SetPlayerInput(PlayerInputController playerInputController)
    {
        playerInput = playerInputController.playerInput;
    }
}
