using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("# 입력 및 이동")]
    private Vector2 inputVec; // 입력 벡터 (방향)

    public Vector2 lastInputVec; // 마지막 입력 벡터
    
    [HideInInspector]public float lastInputX = 1f; // 마지막 수평 입력 (0이 아닌 값), 기본값 1(오른쪽)
    public float baseSpeed = 6f;
    public float currentSpeed = 3f; // 이동 속도

    private Animator anim; // 애니메이터
    private SpriteRenderer spriter; // 스프라이트 렌더러
    private Rigidbody2D rigid; // 리지드바디
    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        lastInputVec = Vector2.right;
    }

    private void Update()
    {
        if (!GameManager.instance.isGameActive)
            return;
        UpdateLastInputVector();
        SetAnimation();
    }
   
    private void FixedUpdate()
    {
        if (!GameManager.instance.isGameActive)
            return;
        Move();
    }

    // 입력 벡터 업데이트
    private void UpdateLastInputVector()
    {
        if (inputVec != Vector2.zero)
        {
            lastInputVec = inputVec.normalized;
        }

        if (inputVec.x != 0f)
        {
            lastInputX = inputVec.x;
        }
    }
    // 플레이어 이동
    private void Move()
    {
        Vector2 nextVec = inputVec.normalized * currentSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    // 애니메이션 세팅
    private void SetAnimation()
    {
        // Animator 세팅
        anim.SetFloat("Speed", inputVec.magnitude);
        // flipX 관리
        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    // 플레이어 이동 옵션 초기화
    public void InitPlayerMoveOption()
    {
        baseSpeed = baseSpeed * GameManager.instance.playerData.speedMult; // 플레이어 기본 이동속도 적용
        currentSpeed = baseSpeed;
    }
       
    private void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
}
