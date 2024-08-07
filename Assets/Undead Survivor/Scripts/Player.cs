using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;    
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon; // 플레이어 애니메이터 관리

    private SpriteRenderer spriter;
    private Rigidbody2D rigid;
    private Animator anim;
    private Color hitColor;
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
        hitColor = new Color(1, 0.4198f, 0.4198f);
    }
    private void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];   
    }
    private void Update()
    {
        if (GameManager.instance.isLive)
        {
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isLive)
        {
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }
    }

    private void LateUpdate()
    {
        if (GameManager.instance.isLive)
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (GameManager.instance.isLive && collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.instance.health -= Time.deltaTime * 10;
            spriter.color = hitColor;
            if (GameManager.instance.health < 0)
            {
                for (int index = 2; index < transform.childCount; index++)
                {
                    transform.GetChild(index).gameObject.SetActive(false);
                }

                anim.SetTrigger("Dead");
                GameManager.instance.GameOver();
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (GameManager.instance.isLive && collision.gameObject.CompareTag("Enemy"))
        {
            spriter.color = Color.white;
        }
    }
}
