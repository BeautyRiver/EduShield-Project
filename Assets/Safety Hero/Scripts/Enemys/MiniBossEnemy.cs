using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossEnemy : Enemy
{
    protected override void DropReward()
    {        
        GameObject reward = gm.poolManager.Get(PoolObjectType.BoxReward); // 보상 상자 생성
        reward.transform.position = transform.position;
    }

    protected override void FlipX()
    {
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }

    protected override void Move()
    {
        Vector2 dirVec = targetRb.position - rigid.position; // 타겟 방향
        nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
}
