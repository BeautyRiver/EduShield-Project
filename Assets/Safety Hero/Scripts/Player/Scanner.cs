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
            Exp expObj = item.GetComponent<Exp>();
            if (!expObj.IsMoving)
                expObj.ItemMoveLogic(transform);            
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
