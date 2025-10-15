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
    public PlayerMove playerMove;
    private SpriteRenderer spriter;
    public Rigidbody2D rigid { get; private set; }
    private Animator anim;
    private CapsuleCollider2D col;

    private void Awake()
    {
        // Character Model 자식에서 가져오기
        spriter = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        playerMove = GetComponent<PlayerMove>();
    }

    // 플레이어 초기화
    public void PlayerInit(PlayerData playerData)
    {

        this.playerId = playerData.characterId; // 플레이어 ID 설정
        health = maxHealth;
        anim.runtimeAnimatorController = playerData.animCon;
    }  





}


