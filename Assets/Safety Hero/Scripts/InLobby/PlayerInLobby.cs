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
    private TargetScanner scanner;
    private Animator anim;


    // 최근 가장 가까운 npc
    [SerializeField] private Npc recentlyNearestNpc;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        scanner = GetComponent<TargetScanner>();
        anim = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        UpdateNearestNpc();
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

    private void UpdateNearestNpc()
    {
        Transform nearstTarget = scanner.nearestTarget;

        if (nearstTarget != null)
        {
            if (recentlyNearestNpc == null || recentlyNearestNpc.transform != nearstTarget)
            {
                // (가장 가까운 npc가 바뀌었음) 이전 npc의 말풍선 숨기기 
                if (recentlyNearestNpc != null)
                {
                    recentlyNearestNpc.ShowSpeechBubble(false);
                }

                recentlyNearestNpc = nearstTarget.GetComponent<Npc>();
                recentlyNearestNpc.ShowSpeechBubble(true);
            }

            recentlyNearestNpc.LookAtPlayer(transform.position);
        }
        else
        {
            if (recentlyNearestNpc != null)
            {
                recentlyNearestNpc.ShowSpeechBubble(false);
                recentlyNearestNpc = null;
            }
        }
    }  
    public Npc GetCurrentTargetNpc()
    {
        return recentlyNearestNpc;
    }
}


