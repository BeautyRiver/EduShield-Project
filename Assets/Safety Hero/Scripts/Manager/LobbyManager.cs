using DarkTonic.MasterAudio;
using Unity.Cinemachine;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    
    public PlayerInLobby player;
    [SerializeField] private PlayerData playerData; // 원본

    [Header("플레이어 참조")]
    [SerializeField] private PlayerInputController playerInputController;

    [Header("카메라 참조")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera vcamGameplay;
    [SerializeField] private CinemachineCamera vcamInteract;
    private LayerMask viewOrignalLayer;
    [SerializeField] private LayerMask viewExceptionLayer;

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
        MasterAudio.StartPlaylist("Lobby");
        player.PlayerInit(playerData);
        playerInputController = player.GetComponent<PlayerInputController>();
        viewOrignalLayer = mainCamera.cullingMask;
    }

  
    //public void ChangePlayerState(LobbyState newState)
    //{
    //    if (playerState == LobbyState.Interacting && newState != LobbyState.Interacting)
    //        DeInteractingCamera();

    //    playerState = newState;
    //    switch (playerState)
    //    {
    //        case LobbyState.FreeMoving:                
    //            //Debug.Log("로비에서 자유 이동 모드로 전환되었습니다.");
    //            playerInputController.SwitchActionMap("InLobby");

    //            break;

    //        case LobbyState.Interacting:                
    //            //Debug.Log("로비에서 상호작용 모드로 전환되었습니다.");
    //            playerInputController.SwitchActionMap("UI");
    //            playerInputController.StopMovement(); // 플레이어 강제 정지

    //            break;
    //    }
    //}
    
    public void InteractingCamera()
    {
        vcamInteract.Priority = 20;
        mainCamera.cullingMask = viewOrignalLayer & ~viewExceptionLayer;
    }

    public void DeInteractingCamera()
    {
        vcamInteract.Priority = 5;
        mainCamera.cullingMask = viewOrignalLayer;
    }   
}
