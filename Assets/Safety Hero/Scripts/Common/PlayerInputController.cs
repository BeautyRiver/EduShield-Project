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
    private GlobalManager globalManager;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
        playerInteraction = GetComponent<PlayerInteraction>();
    }
    private void Start()
    {
        globalManager = GlobalManager.instance;
        globalManager.ChangePlayerState(PlayerState.FreeMove);
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
        if (globalManager.playerState != PlayerState.FreeMove) return; // 자유 이동 상태에서만 상호작용 가능

        playerInteraction.RequestInteraction();
    }


    public string GetCurrentActionMap()
    {
        return playerInput.currentActionMap.name;
    }

}
