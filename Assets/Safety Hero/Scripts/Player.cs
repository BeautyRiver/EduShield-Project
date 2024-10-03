using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("입력 및 이동")]
    public Vector2 inputVec; // 입력 벡터 (방향)
    public Vector2 lastInputVec = new Vector2(1f,0f);
    public float lastXInputVec = 1f;  // 마지막 x축 방향만 기억
    public float speed = 3f; // 이동 속도


    [Header("게임 오브젝트 참조")]
    public Scanner scanner; // 적 탐색기
    public Hand[] hands; // 플레이어 무기 (손) 배열    
    [HideInInspector] public Spawner spawner;

    [Header("애니메이션")]
    [SerializeField] private List<PlayerAnimatorControll> animCon; // 플레이어 애니메이터 컨트롤러

    [Header("피격 관리")]
    private Color hitColor; // 피격 시 색상
    private Color normalColor; // 기본 색상
    private WaitForSeconds hitingTime; // 피격 지속 시간
    private bool isHiting; // 피격 중 여부

    // 기타 컴포넌트
    private SpriteRenderer spriter;
    private Rigidbody2D rigid; 
    private Animator anim;
    private GameManager gm; // 게임 매니저 참조
    private CapsuleCollider2D col;
    private bool isTransforming;

    private void Awake()
    {        
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
        spawner = GetComponentInChildren<Spawner>(true);
        col = GetComponent<CapsuleCollider2D>(); 
        hitColor = new Color(0.86f, 0.2f, 0.2f);
        normalColor = spriter.color;
        hitingTime = new WaitForSeconds(0.2f);
    }

    private void Start()
    {
        gm = GameManager.instance;              
    }

    private void Update()
    {
        if (gm.isLive)
        {
            // 입력 벡터 설정
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");

            if (inputVec != Vector2.zero)
            {
                lastInputVec = inputVec;

                // x축이 0이 아닐 때만 마지막 x축 방향을 저장
                if (inputVec.x != 0)
                {
                    lastXInputVec = inputVec.x;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (gm.isLive)
        {
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }
    }

    private void LateUpdate()
    {
        if (gm.isLive)
        {
            // Animator 세팅
            anim.SetFloat("Speed", inputVec.magnitude);
            // flipX 관리
            if (inputVec.x != 0)
            {
                spriter.flipX = inputVec.x < 0;
            }
        }
    }

    // 물리 충돌 일어날 때
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 플레이어가 생존중이 아니라면 실행 X
        if (gm.isLive == false)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            gm.health -= Time.deltaTime * collision.gameObject.GetComponent<Enemy>().damage;

            // 플레이어 피격색상 변경
            if (!isHiting && !isTransforming)
                StartCoroutine(HitColor());

            if (gm.health < 0)
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
        if (gm.isLive == false)
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
    public IEnumerator TransformationColor(int idx)
    {
        isTransforming = true;
        gm.GenerateEffect(1, transform);
        Vector3 originalScale = transform.localScale;

        transform.DOScale(originalScale * 1.2f, 0.05f).OnComplete(() =>
        {
            spriter.DOFade(0.2f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                anim.runtimeAnimatorController = animCon[gm.playerId].runAniCon[idx];
            });
            transform.DOScale(originalScale, 0.1f);
        });
        yield return new WaitForSeconds(0.2f);

        isTransforming = false;
    }

    // 플레이어 초기화
    public void PlayerInit()
    {
        speed = speed * gm.playerData.speedMult; // 플레이어 기본 이동속도 적용
        anim.runtimeAnimatorController = animCon[gm.playerId].runAniCon[0];
        Debug.Log($"애니메이션 컨트롤러 변경 {gm.playerId}");
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


