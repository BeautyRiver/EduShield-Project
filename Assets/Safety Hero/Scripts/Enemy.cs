using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [Header("적 상태")]
    public float speed;
    public float health;
    public float maxHealth;
    private bool isLive;

    [Header("참조")]
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    private Collider2D coll;
    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;
    private WaitForFixedUpdate wait;
    private SortingGroup sortingGroup;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
        wait = new WaitForFixedUpdate();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isLive && isLive)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                return;

            Vector2 dirVec = target.position - rigid.position; // 타겟 방향
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.velocity = Vector2.zero;
        }
        else
            return;
    }

    private void LateUpdate()
    {
        if (GameManager.instance.isLive && isLive)
        {
            spriter.flipX = target.position.x < rigid.position.x;
        }
        else
            return;
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        // 초기화
        sortingGroup.sortingOrder = 1;
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && isLive)
        {
            health -= collision.GetComponent<Bullet>().damage; // 체력 감소
            StartCoroutine(KnockBack()); // 넉백
            anim.SetTrigger("Hit"); // 맞는 애니메이션 재생
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit); // 음향재생

            if (health <= 0) // 체력 0 이하 사망
            {
                isLive = false;
                coll.enabled = false; // 콜라이더 끄기
                rigid.simulated = false;
                spriter.sortingOrder = 1;
                anim.SetBool("Dead", true);

                GameManager.instance.kill++;
                GameManager.instance.GetExp(1);

                if (GameManager.instance.isLive)
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead); // 음향재생
            }                       
        }
        else
            return;
    }

    private IEnumerator KnockBack()
    {
        yield return wait; // 다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = target.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 1.5f, ForceMode2D.Impulse);
    }

    public void SetOrderLayer()
    {
        sortingGroup.sortingOrder = 0;
    }
    private void Dead()
    {
        gameObject.SetActive(false);
    }
}
