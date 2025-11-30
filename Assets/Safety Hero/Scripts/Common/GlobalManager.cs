using UnityEngine;

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


}
