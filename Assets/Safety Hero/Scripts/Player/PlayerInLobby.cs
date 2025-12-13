using UnityEngine;
using VInspector;

public class PlayerInLobby : MonoBehaviour
{
    [Header("# 플레이어 정보")]
    [ReadOnly] public int playerId; // 플레이어 ID
    public float health; // 현재 체력
    public float maxHealth = 100; // 최대 체력  

    // 기타 컴포넌트
    [HideInInspector]
    private PlayerMove playerMove;
    private Animator anim;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        anim = GetComponentInChildren<Animator>();

    }

    // 플레이어 초기화
    public void PlayerInit(PlayerData playerData)
    {
        SetLobbyState();
        this.playerId = playerData.characterId; // 플레이어 ID 설정
        health = maxHealth;
        anim.runtimeAnimatorController = playerData.animCon;
    }

    // 로비 상태 설정
    public void SetLobbyState()
    {
        playerMove.SetCurretSpeed(playerMove.baseSpeed);
    }

    
}


