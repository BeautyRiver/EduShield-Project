using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossEnemy : Enemy, IMovable
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
        FlipX();
    }

    protected override void DropReward()
    {        
        GameObject reward = PoolManager.instance.Get(myData.dropRewardItemPrefab); // 보상 상자 생성
        reward.transform.position = transform.position;
    }

    protected override void FlipX()
    {
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }

    public void Move()
    {
        dirVec = targetRb.position - rigid.position; // 타겟 방향
        nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
}
