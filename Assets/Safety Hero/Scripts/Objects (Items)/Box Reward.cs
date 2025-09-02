using DarkTonic.MasterAudio;
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
            if (arrowAni.gameObject.activeSelf)
            {
                MasterAudio.PlaySound("RewardBoxOpen");
            }
            arrowAni.gameObject.SetActive(false);
            anim.SetBool("Open", true);
            Debug.Log("상자 오픈~");
            StartCoroutine(ShowReward());
        }
    }

    private IEnumerator ShowReward()
    {
        int count = Random.Range(20, 60);
        for (int i = 0; i < count; i++)
        {            
            GameObject expObj = GameManager.instance.poolManager.Get(PoolType.Drop, 0); // exp 소환
            CircleCollider2D coll = expObj.GetComponent<CircleCollider2D>();
            coll.enabled = false;
            expObj.GetComponent<Exp>().exp = Random.Range(1, GameManager.instance.spawner.level+1);
            expObj.transform.position = transform.position;

            Vector2 randomDir = new Vector2(Random.Range(-0.7f, 0.7f), Random.Range(-0.2f, 0.2f)); // 좌우로만 튀어나가게 설정

            // 무작위 높이와 거리 설정
            float jumpPower = Random.Range(2f, 3f); // 위로 튀어오를 힘 (점프 높이)
            float randomDistance = Random.Range(2f, 4f);  // 이동할 거리

            // DOTween으로 점프 애니메이션: 좌우 방향으로 randomDistance만큼 점프
            expObj.transform.DOJump((Vector2)transform.position + randomDir * randomDistance, jumpPower, 1, 1f)
                .SetEase(Ease.OutQuad).OnComplete(() => { coll.enabled = true; });
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);
    }

    private void Dead()
    {
        gameObject.SetActive(false);
    }
}
