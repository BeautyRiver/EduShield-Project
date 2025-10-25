using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerInputController : MonoBehaviour
{
    private PlayerMove playerMove;
    private PlayerInput playerInput;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
    }

    // InGame or Lobby
    private void OnMove(InputValue value)
    {
        if (playerMove == null) return;

        playerMove.SetMoveDirection(value.Get<Vector2>());
    }

    private void OnInteract(InputValue value)
    {
        if (LobbyManager.instance == null) return;

        Debug.Log("상호작용 입력 감지됨.");

        LobbyManager.instance.RequestInteraction();
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

}
