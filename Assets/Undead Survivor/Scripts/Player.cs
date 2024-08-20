using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed = 3f;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon; // 플레이어 애니메이터 관리

    private SpriteRenderer spriter;
    private Rigidbody2D rigid;
    private Animator anim;

    // 플레이어 피격 관리
    private Color hitColor;
    private Color normalColor;
    private WaitForSeconds hitingTime;
    private bool isHiting;

    private GameManager gameManager;
    private void Awake()
    {        
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
        hitColor = new Color(1, 0.42f, 0.42f);
        normalColor = spriter.color;

        hitingTime = new WaitForSeconds(0.2f);
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        speed = speed * gameManager.playerData.speedMult; // 플레이어 기본 이동속도 적용
        anim.runtimeAnimatorController = animCon[gameManager.playerId];
    }
    private void OnEnable()
    {
        
    }
    private void Update()
    {
        if (gameManager.isLive)
        {
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.isLive)
        {
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }
    }

    private void LateUpdate()
    {
        if (gameManager.isLive)
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
        // 플레이어가 생존중이 아니라면 실행 X
        if (gameManager.isLive == false)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameManager.health -= Time.deltaTime * 10;

            // 플레이어 피격색상 변경
            if (!isHiting)
                StartCoroutine(ColorChangeCol());

            if (gameManager.health < 0)
            {
                for (int index = 2; index < transform.childCount; index++)
                {
                    transform.GetChild(index).gameObject.SetActive(false);
                }

                anim.SetTrigger("Dead");
                gameManager.GameOver();
            }
        }        
    }

    private IEnumerator ColorChangeCol()
    {
        isHiting = true;
        spriter.color = hitColor;
        yield return hitingTime;
        spriter.color = normalColor;
        isHiting = false;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 플레이어가 생존중이 아니라면 실행 X
        if (gameManager.isLive == false)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            spriter.color = normalColor;
        }
    }
}
