using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("# 플레이어 정보")]
    public int playerId; // 플레이어 ID
    public float health; // 현재 체력
    public float maxHealth = 100; // 최대 체력  

    [Header("# 게임 오브젝트 참조")]
    public Scanner scanner; // 적 탐색기        
    [HideInInspector] public Spawner spawner;

    [Header("# 애니메이션")]
    [SerializeField] private List<PlayerAnimatorControll> animCon; // 플레이어 애니메이터 컨트롤러

    [Header("# 피격 관리")]
    [ColorUsage(true, true)]
    [SerializeField] private Color hitColor; // 피격 시 색상

    private Color normalColor; // 기본 색상

    [ColorUsage(true, true)]
    [SerializeField] private Color[] transformingColor; // 변신 색상들

    private WaitForSeconds hitingTime; // 피격 지속 시간
    private bool isHiting; // 피격 중 여부
    private bool isTransforming; // 변신 중 여부

    // 기타 컴포넌트
    [HideInInspector]
    public PlayerMove playerMove;

    private SpriteRenderer spriter;
    public Rigidbody2D rigid { get; private set; } 
    private Animator anim;
    private GameManager gm; // 게임 매니저 참조
    private CapsuleCollider2D col;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        spawner = GetComponentInChildren<Spawner>(true);
        col = GetComponent<CapsuleCollider2D>();
        playerMove = GetComponent<PlayerMove>();
        normalColor = spriter.color;
        hitingTime = new WaitForSeconds(0.2f);
    }

    private void Start()
    {
        gm = GameManager.instance;              
    }
   

    // 물리 충돌 일어날 때
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 플레이어가 생존중이 아니라면 실행 X
        if (gm.isGameActive == false)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            health -= Time.deltaTime * collision.gameObject.GetComponent<Enemy>().damage;

            // 플레이어 피격색상 변경
            if (!isHiting && !isTransforming)
                StartCoroutine(HitColor());

            if (health < 0)
            {
                for (int index = 2; index < transform.childCount; index++)
                {
                    transform.GetChild(index).gameObject.SetActive(false);
                }
                PlayerDead();
            }
        }        
    }

    // 물리 충돌 벗어날 때
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 플레이어가 생존중이 아니라면 실행 X
        if (gm.isGameActive == false)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            spriter.color = normalColor;
        }
    }

    // 피격 색상 변경 코루틴
    private IEnumerator HitColor()
    {
        isHiting = true;
        spriter.color = hitColor;
        yield return hitingTime;
        spriter.color = normalColor;
        isHiting = false;
    }

    // 변신중 색상 변경 코루틴
    public IEnumerator TransformationColor(int typeIdx)
    {
        typeIdx += 1; // typeindex보다 1 크게 (타입이 -1<기본타입> 부터 시작해서)
        isTransforming = true;
        gm.GenerateEffect(0, transform, transformingColor[typeIdx]); // 플레이어 이팩트 생성 시키기
        Vector3 originalScale = transform.localScale;

        transform.DOScale(originalScale * 1.2f, 0.05f).OnComplete(() =>
        {
            spriter.DOFade(0.2f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                anim.runtimeAnimatorController = animCon[playerId].runAniCon[typeIdx]; 
            });
            transform.DOScale(originalScale, 0.1f);
        });
        yield return new WaitForSeconds(0.2f);

        isTransforming = false;
    }

    // 플레이어 초기화
    public void PlayerInit(int playerId)
    {
        this.playerId = playerId; // 플레이어 ID 설정
        health = maxHealth * gm.playerData.maxHpMult; // 플레이어 체력 세팅 
        playerMove.InitPlayerMoveOption(); // 플레이어 이동 옵션 초기화
        anim.runtimeAnimatorController = animCon[playerId].runAniCon[0];
        Debug.Log($"애니메이션 컨트롤러 변경 {playerId}");
    }

    public void PlayerDead()
    {
        col.enabled = false;
        anim.SetTrigger("Dead");
        gm.GameOver();
    }

    [System.Serializable]
    public class PlayerAnimatorControll
    {
        public RuntimeAnimatorController[] runAniCon;
    }


}


