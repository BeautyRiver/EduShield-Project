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
    private PlayerInput playerInput;
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

        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
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
                SwitchActionMap("InGame"); // 인게임 움직임 활성화
                Time.timeScale = nowTimeScale;
                break;

            case GameState.Paused:
                SwitchActionMap("UI"); // UI 입력 활성화
                Time.timeScale = 0f;
                break;

            case GameState.LevelUp:
                SwitchActionMap("UI"); // UI 입력 활성화
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                SwitchActionMap("Empty"); // 입력 비활성화
                Time.timeScale = 0f;
                StartCoroutine(GameManager.instance.GameOverRoutine());
                break;

            case GameState.Victory:
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
        playerInput.SwitchCurrentActionMap(mapName);
    }
}
