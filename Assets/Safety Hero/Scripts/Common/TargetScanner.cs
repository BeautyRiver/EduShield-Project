using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetScanner : MonoBehaviour
{
    [Header("# 스캔 (범위 : 사거리)")]
    public float scanRange;
    [SerializeField] private Color scanColor;
    [SerializeField] private LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget { get; private set; }

    private void Start()
    {
        StartCoroutine(FindNearstTarget());
    }

    IEnumerator FindNearstTarget()
    {
        while (true)
        {
            if (GameManager.instance.currentState == GameState.Playing)
            {
                targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
                nearestTarget = GetNearest();
                yield return null;
            }
            else
                yield return null;
        }
        
    }
    // 가장 가까운 대상 반환 함수
    public Transform GetNearest()
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
    
    private void OnDrawGizmos()
    {
        Gizmos.color = scanColor;
        Gizmos.DrawWireSphere(transform.position, scanRange);
    }
}
