using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : Enemy, IRepositionable, IKnockBackable, IMovable
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
        FlipX();
    }

    protected override void DropReward()
    {
        GameObject expObj = GameManager.instance.poolManager.Get(PoolType.Item, 0); // expCount 생성
        expObj.transform.position = transform.position;
        expObj.GetComponent<Exp>().exp = this.exp;
    }

    protected override void FlipX()
    {
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }

    public void Move()
    {
        Vector2 dirVec = targetRb.position - rigid.position; 
        nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
}
