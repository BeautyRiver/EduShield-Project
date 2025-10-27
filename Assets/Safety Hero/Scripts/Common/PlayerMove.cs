using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [Header("# 이동 속도")]
    [field: SerializeField] public float baseSpeed { get; private set; }
    [field: SerializeField] public float currentSpeed { get; private set; }

    [Header("# 마지막 입력 방향")]
    public Vector2 lastInputVec { get; private set; }   // 마지막 입력 벡터
    [HideInInspector] public float lastInputX = 1f;     // 마지막 입력 X 방향 (왼쪽: -1, 오른쪽: 1)

    private Vector2 moveDirection; // 'inputVec' 대신 'moveDirection' 사용
    private Animator anim;
    private SpriteRenderer spriter;
    private Rigidbody2D rigid;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        spriter = GetComponentInChildren<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        lastInputVec = Vector2.right;
    }

    private void Update()
    {
        UpdateLastInputVector();
        SetAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }
    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void SetCurretSpeed(float speed)
    {
        currentSpeed = speed;
        //Debug.Log($"currentSpeed: {speed}");
    }

    // 입력 벡터 업데이트
    private void UpdateLastInputVector()
    {
        if (moveDirection != Vector2.zero)
        {
            lastInputVec = moveDirection;
        }

        if (moveDirection.x != 0f)
        {
            lastInputX = lastInputVec.x;
        }
    }
    // 플레이어 이동
    private void Move()
    {
        Vector2 nextVec = moveDirection * currentSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void SetAnimation()
    {
        anim.SetFloat("Speed", moveDirection.magnitude);
        if (moveDirection.x != 0)
        {
            spriter.flipX = moveDirection.x < 0;
        }
    }


    //private void OnMove(InputValue value)
    //{
    //    inputVec = value.Get<Vector2>();
    //}


}
