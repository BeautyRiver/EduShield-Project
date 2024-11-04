using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : Enemy, IRepositionable
{
    protected override void DropReward()
    {
        GameObject exp = GameManager.instance.poolManager.Get(PoolObjectType.Exp); // exp 소환
        exp.transform.position = transform.position;
        exp.GetComponent<Exp>().exp = this.exp;
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
