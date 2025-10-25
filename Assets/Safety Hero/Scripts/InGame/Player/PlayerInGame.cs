using System.Collections;
using UnityEngine;
using VInspector;

public class PlayerInGame : MonoBehaviour, IDamageable
{
    [Header("# 플레이어 정보")]
    [ReadOnly] public int playerId; // 플레이어 ID
    public float health; // 현재 체력
    public float maxHealth = 100; // 최대 체력  

    [Header("# 게임 오브젝트 참조")]
    public TargetScanner scanner; // 적 탐색기        


    [Header("# 이펙트 관리")]
    [ColorUsage(true, true)]
    [SerializeField] private Color hitColor; // 피격 시 색상
    [SerializeField] private GameObject playerEffect;

    private Color normalColor; // 기본 색상

    private WaitForSeconds hitingTime; // 피격 지속 시간
    private bool isHiting; // 피격 중 여부

    // 기타 컴포넌트
    [HideInInspector]
    public PlayerMove playerMove;

    private SpriteRenderer spriter;
    private Rigidbody2D rigid;
    private Animator anim;
    private GameManager gm; // 게임 매니저 참조
    private CapsuleCollider2D col;
    
    [Header("# For Debug")]
    [Foldout("Debugging용")]
    public bool isInvincible;
    [EndFoldout]

    private void Awake()
    {
        // Character Model 자식에서 가져오기
        spriter = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        rigid = GetComponent<Rigidbody2D>();
        scanner = GetComponent<TargetScanner>();
        col = GetComponent<CapsuleCollider2D>();
        playerMove = GetComponent<PlayerMove>();
        normalColor = spriter.color;
        hitingTime = new WaitForSeconds(0.2f);

        gm = GameManager.instance;

    }

    // 물리 충돌 일어날 때
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 플레이어가 생존중이 아니라면 실행 X
        if (gm.currentState != GameState.Playing)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.TryGetComponent(out Enemy enemy))
            {
                DamagedLogic(collision.collider, enemy.damage);
                if (!isHiting)
                {
                    isHiting = true;
                    spriter.color = hitColor;
                }
            }
        }        
    }

    // 물리 충돌 벗어날 때
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 플레이어가 생존중이 아니라면 실행 X
        if (gm.currentState != GameState.Playing)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isHiting)
            {
                isHiting = false;
                spriter.color = normalColor;
            }
        }
    }
   

    // 플레이어 초기화
    public void PlayerInit(PlayerData playerData)
    {
        this.playerId = playerData.characterId; // 플레이어 ID 설정
        health = maxHealth;
        RecalculateStats();
        anim.runtimeAnimatorController = playerData.animCon;
    }

    public void RecalculateStats()
    {           
        // 체력
        float oldMaxHealth = maxHealth;
        //maxHealth = 100 * gm.playerData.maxHpMult; // 100은 기본체력
        if (maxHealth > oldMaxHealth) // 최대 체력이 증가했다면
        {
            health += maxHealth - oldMaxHealth; // 그만큼 체력 회복도 시켜주고..
        }
        health = Mathf.Min(health, maxHealth);

        // 이동 속도
        RecalculateSpeed();
    }

    // 속도 재계산
    public void RecalculateSpeed()
    {
        playerMove.SetCurretSpeed(playerMove.baseSpeed * gm.playerData.speedMult);
    }
    public void PlayerDead()
    {
        col.enabled = false;
        anim.SetTrigger("Dead");
        gm.ChangeState(GameState.GameOver);
    }

    public void DamagedLogic(Collider2D collision, float damage)
    {
        health -= Time.deltaTime * damage;
        if (health < 0)
        {
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
            PlayerDead();
        }
    }

    // 이펙트 생성시키기
    public void PlayerGenerateEffect()
    {
        GameObject effect = PoolManager.instance.Get(playerEffect); // 플레이어 힐 이펙트
        effect.transform.parent = transform;
        effect.transform.localPosition = Vector3.zero;        
    }

  

}


