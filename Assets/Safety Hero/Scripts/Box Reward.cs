using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxReward : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D coll;
    private Transform arrowAni;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        arrowAni = transform.GetChild(0);
    }
    private void OnEnable()
    {
        arrowAni.gameObject.SetActive(true);
        anim.SetBool("Open", false);
        coll.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            arrowAni.gameObject.SetActive(false);
            anim.SetBool("Open", true);
            Debug.Log("상자 오픈~");
            StartCoroutine(ShowReward());
        }
    }

    private IEnumerator ShowReward()
    {
        int count = Random.Range(25, 60);
        for (int i = 0; i < count; i++)
        {
            GameObject exp = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 1);
            exp.GetComponent<Exp>().exp = Random.Range(1, GameManager.instance.player.spawner.level+1);
            exp.transform.position = transform.position;

            Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), 0); // 좌우로만 튀어나가게 설정

            // 무작위 높이와 거리 설정
            float jumpPower = Random.Range(2f, 3f); // 위로 튀어오를 힘 (점프 높이)
            float randomDistance = Random.Range(2f, 5f);  // 이동할 거리

            // DOTween으로 점프 애니메이션: 좌우 방향으로 randomDistance만큼 점프
            exp.transform.DOJump((Vector2)transform.position + randomDir * randomDistance, jumpPower, 1, 0.5f)
                .SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);
    }

    private void Dead()
    {
        gameObject.SetActive(false);
    }
}
