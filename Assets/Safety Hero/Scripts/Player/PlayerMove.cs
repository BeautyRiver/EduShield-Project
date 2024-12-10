using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("# 입력 및 이동")]
    private Vector2 inputVec; // 입력 벡터 (방향)
    [SerializeField]
    public Vector2 Vector2 { get => inputVec; }

    public Vector2 lastInputVec = new Vector2(1f, 0f);
    public float lastXInputVec = 1f;  // 마지막 x축 방향만 기억
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
    }

    private void Update()
    {
        if (!GameManager.instance.isGameActive)
            return;
        UpdateInputVector();
        SetAnimation();
    }
   

    private void FixedUpdate()
    {
        if (!GameManager.instance.isGameActive)
            return;
        MovePlayer();
    }

    // 입력 벡터 업데이트
    private void UpdateInputVector()
    {
        if (inputVec != Vector2.zero)
        {
            lastInputVec = inputVec;
        }
    }
    // 플레이어 이동
    private void MovePlayer()
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
