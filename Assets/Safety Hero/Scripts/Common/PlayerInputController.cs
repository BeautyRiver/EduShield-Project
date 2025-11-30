using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerInteraction))]

public class PlayerInputController : MonoBehaviour
{
    [HideInInspector] public PlayerMove playerMove;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public PlayerInteraction playerInteraction;
    private GlobalManager glM;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
        playerInteraction = GetComponent<PlayerInteraction>();
    }
    private void Start()
    {
        glM = GlobalManager.instance;
        ChangeState(PlayerState.FreeMove);
    }

    public void ChangeState(PlayerState newState)
    {
        if (newState == PlayerState.InUI)
        {
            StopMovement();
        }

        glM.playerState = newState;
        switch (glM.playerState)
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

    // InGame or Lobby
    private void OnMove(InputValue value)
    {
        if (playerMove == null) return;

        playerMove.SetMoveDirection(value.Get<Vector2>());
    }

    private void OnInteract(InputValue value)
    {
        if (playerInteraction == null) return;
        if (glM.playerState != PlayerState.FreeMove) return; // 자유 이동 상태에서만 상호작용 가능

        Debug.Log("상호작용 입력 감지됨.");
        playerInteraction.RequestInteraction();
    }

    public void SwitchActionMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);             
        Debug.Log(mapName + " 액션 맵으로 전환되었습니다.");
    }

    public void StopMovement()
    {
        playerMove.SetMoveDirection(Vector2.zero);
    }

    public string GetCurrentActionMap()
    {
        return playerInput.currentActionMap.name;
    }

}
