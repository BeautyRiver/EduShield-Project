using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    
    public PlayerInLobby player;
    [SerializeField] private PlayerData playerData; // 원본

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        player.PlayerInit(playerData);
    }
}
