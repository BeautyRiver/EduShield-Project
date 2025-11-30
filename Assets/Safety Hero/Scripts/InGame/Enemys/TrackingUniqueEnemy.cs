using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingUniqueEnemy : Enemy, IMovable
{
    [Header("# 유니크 몬스터 설정")]
    public float trackingTime = 1.5f; // 처음 몇 초 추적할지
    public float turnSpeed = 2f; // 낮을수록 턴이 굼뜸
    private Vector2 currentMoveDir; 
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
        FlipX();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || !coll.CompareTag("Ground"))
            return;

        gameObject.SetActive(false);
    }

    protected override void FlipX()
    {
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        currentMoveDir = (targetRb.position - rigid.position).normalized;

        StopAllCoroutines();
        StartCoroutine(TargetDirectionCoroutine());
    }

    private IEnumerator TargetDirectionCoroutine()
    {
        float timer = 0f;
        while (timer < trackingTime)
        {
            // 플레이어가 살아있을 때만 목표 방향을 계산
            if (gm.player != null && GlobalManager.instance.gameState != GameState.Playing)
            {
                // 목표 방향 (플레이어 방향) 계산
                Vector2 targetDirection = (targetRb.position - rigid.position).normalized;
                // 현재 방향에서 목표 방향으로 서서히 방향 전환 (Lerp 사용)
                currentMoveDir = Vector2.Lerp(currentMoveDir, targetDirection, turnSpeed * Time.deltaTime);
            }

            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기 (Update 주기)
        }
    }

    public void Move()
    {
        if (!isLive) return;
        rigid.MovePosition(rigid.position + currentMoveDir * speed * Time.fixedDeltaTime);
    }
}