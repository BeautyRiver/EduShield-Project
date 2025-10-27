using Unity.Cinemachine;
using UnityEngine;

public enum LobbyState
{
    FreeMoving,   // 자유 이동 모드
    Interacting   // Npc 대화, or UI 상호작용 모드
}

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    
    public PlayerInLobby player;
    public LobbyState currentState;
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
        player.PlayerInit(playerData);
        ChangeState(LobbyState.FreeMoving);
        playerInputController = player.GetComponent<PlayerInputController>();
        viewOrignalLayer = mainCamera.cullingMask;
    }

    public void RequestInteraction()
    {
        if (currentState != LobbyState.FreeMoving) return; // 이미 상호작용 중이면 무시, 움직일 때만 상호작용 가능

        Npc targetNpc = player.GetCurrentTargetNpc();

        // Npc와 상호작용 시작
        if (targetNpc != null)
        {
            ChangeState(LobbyState.Interacting);
            targetNpc.Interaction(); // Npc 상호작용 시작
            InteractingCamera();
        }
        else
        {
            Debug.Log("상호작용할 Npc가 없습니다.");
        }
    }
    public void ChangeState(LobbyState newState)
    {
        if (currentState == LobbyState.Interacting && newState != LobbyState.Interacting)
            DeInteractingCamera();

        currentState = newState;
        switch (currentState)
        {
            case LobbyState.FreeMoving:                
                //Debug.Log("로비에서 자유 이동 모드로 전환되었습니다.");
                playerInputController.SwitchActionMap("InLobby");

                break;

            case LobbyState.Interacting:                
                //Debug.Log("로비에서 상호작용 모드로 전환되었습니다.");
                playerInputController.SwitchActionMap("UI");
                playerInputController.StopMovement(); // 플레이어 강제 정지

                break;
        }
    }
    
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
