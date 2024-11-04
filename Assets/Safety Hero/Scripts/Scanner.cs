using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("# 스캔 (범위 : 사거리)")]
    public float scanRange; 
    public LayerMask targetLayer;
    public LayerMask expLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    [Header("# 경험치 획득")]
    public float expCollectionRange = 1f; // 경험치 획득 범위
    public int expValue = 1; // 획득할 경험치 양

    private List<Collider2D> collectedExpItems = new List<Collider2D>(); // 이미 수집된 경험치 아이템 리스트
    private int combinedLayerMask;

    private void Awake()
    {
        combinedLayerMask = targetLayer; // 두 레이어를 함께 검사

    }
    private void FixedUpdate()
    {        
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, combinedLayerMask);        
        nearestTarget = GetNearest();

        // 경험치 아이템 감지 및 획득
        CollectExp();
    }

    // 가장 가까운 대상 반환 함수
    private Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }        

        return result;
    }
    private void CollectExp()
    {
        Collider2D[] expItems = Physics2D.OverlapCircleAll(transform.position, expCollectionRange, expLayer);

        foreach (Collider2D item in expItems)
        {
            Exp exp = item.GetComponent<Exp>();
            if (!exp.isMoving)
            {
                exp.isMoving = true;
                // 아이템 애니메이션
                ItemMoveLogic(item.transform);
            }
        }
    }

    private void ItemMoveLogic(Transform itemTrans)
    {
        // 플레이어와 반대 방향 계산
        Vector2 directionAwayFromPlayer = (itemTrans.position - base.transform.position).normalized;
        Vector2 targetPosition = itemTrans.position + (Vector3)directionAwayFromPlayer * 0.5f;  // 반대 방향으로 약간 이동

        // DOTween을 사용해 플레이어 반대 방향으로 살짝 이동
        itemTrans.DOMove(targetPosition, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // 반대 방향으로 이동이 끝나면 플레이어에게 따라가는 코루틴 시작
            StartCoroutine(FollowPlayer(itemTrans));
        });
    }
    IEnumerator FollowPlayer(Transform itemTrans)
    {
        float closeDistance = 0.1f;  // 플레이어에게 충분히 가까워졌는지 판단할 거리
        while (Vector2.Distance(base.transform.position, itemTrans.position) > closeDistance)
        {
            // 플레이어의 현재 위치를 향해 경험치 아이템이 이동
            Vector2 direction = (base.transform.position - itemTrans.position).normalized;
            itemTrans.Translate(direction * 10f * Time.deltaTime);  // 경험치 이동 속도 조절
            yield return null;  // 다음 프레임까지 대기
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, scanRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, expCollectionRange);
    }
}
